import { Component, inject, OnInit, signal, OnDestroy, afterNextRender, ElementRef, viewChild, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EventService } from '../../../core/services/event.service';
import { TranslateModule } from '@ngx-translate/core';
import { EventCalendarComponent } from '../event-calendar/event-calendar.component';
import type * as LType from 'leaflet';

@Component({
  selector: 'app-event-hub',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule, EventCalendarComponent],
  templateUrl: './event-hub.component.html',
  styleUrls: ['./event-hub.component.scss']
})
export class EventHubComponent implements OnInit, OnDestroy {
  private readonly eventService = inject(EventService);

  private mapContainer = viewChild<ElementRef<HTMLDivElement>>('mapContainer');

  public events = signal<any[]>([]);
  public filteredEvents = signal<any[]>([]);
  public selectedEventId = signal<string | null>(null);
  public selectedCalendarDate = signal<string | null>(null);
  public isLoading = signal<boolean>(true);
  public isMapReady = signal<boolean>(false);
  public currentSort = signal<'date' | 'location'>('date');
  
  private map: LType.Map | undefined;
  private markerMap = new Map<string, LType.Marker>();
  private LeafletLib: any;

  constructor() {
    afterNextRender(async () => {
      const container = this.mapContainer()?.nativeElement;
      if (container && !this.map) {
        this.LeafletLib = await import('leaflet');
        this.initBlankMap(container);
        this.isMapReady.set(true);
      }
    });

    effect(() => {
      const eventList = this.filteredEvents(); 
      const loading = this.isLoading();
      const mapReady = this.isMapReady();

      if (!loading && mapReady && this.map && this.LeafletLib) {
        this.renderMarkers(this.LeafletLib, eventList);
        this.adjustMapBounds(this.LeafletLib, eventList);
      }
    });
  }

  public ngOnInit(): void {
    this.loadEvents();
  }

  public ngOnDestroy(): void {
    if (this.map) {
      this.map.remove();
    }
    this.markerMap.clear();
  }

  private loadEvents(): void {
    this.eventService.getApprovedEvents().subscribe({
      next: (data) => {
        this.events.set(data);
        this.filteredEvents.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  private initBlankMap(container: HTMLDivElement): void {
    this.map = this.LeafletLib.map(container, {
      center: [50.4501, 30.5234],
      zoom: 12,
      dragging: true,
      scrollWheelZoom: true
    });

    this.LeafletLib.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors'
    }).addTo(this.map);

    setTimeout(() => {
      this.map?.invalidateSize();
    }, 200);
  }

  public filterByCalendarDate(dateStr: string): void {
    this.selectedCalendarDate.set(dateStr);
    
    const allEvents = this.events();
    const filtered = allEvents.filter(e => {
      const eDate = new Date(e.startDate);
      const year = eDate.getFullYear();
      const month = String(eDate.getMonth() + 1).padStart(2, '0');
      const day = String(eDate.getDate()).padStart(2, '0');
      const formattedEventDate = `${year}-${month}-${day}`;
      
      return formattedEventDate === dateStr;
    });

    this.filteredEvents.set(filtered);
    this.sortEvents(this.currentSort());
  }

  public resetCalendarFilter(): void {
    this.selectedCalendarDate.set(null);
    this.filteredEvents.set(this.events());
    this.sortEvents(this.currentSort());
  }

  private renderMarkers(L: any, eventList: any[]): void {
    if (!this.map) return;

    this.markerMap.forEach(marker => marker.remove());
    this.markerMap.clear();

    eventList.forEach(event => {
      if (event.latitude === null || event.longitude === null) return;
      
      const lat = Number(event.latitude);
      const lng = Number(event.longitude);
      if (isNaN(lat) || isNaN(lng)) return;

      const markerColor = this.getMarkerColor(event.startDate);
      
      const customIcon = L.divIcon({
        className: 'custom-svg-marker',
        html: `
          <svg width="30" height="42" viewBox="0 0 30 42" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M15 0C6.71 0 0 6.71 0 15C0 26.25 15 42 15 42C15 42 30 26.25 30 15C30 6.71 23.29 0 15 0ZM15 20.25C12.1 20.25 9.75 17.9 9.75 15C9.75 12.1 12.1 9.75 15 9.75C17.9 9.75 20.25 12.1 20.25 15C20.25 17.9 17.9 20.25 15 20.25Z" fill="${markerColor}"/>
          </svg>
        `,
        iconSize: [30, 42],
        iconAnchor: [15, 42],
        popupAnchor: [0, -40]
      });

      const marker = L.marker([lat, lng], { icon: customIcon })
        .addTo(this.map)
        .bindPopup(`
          <div class="map-popup">
            <h4 style="margin:0 0 5px 0; font-size:1rem;">${event.title}</h4>
            <p style="margin:0 0 10px 0; font-size:0.85rem; color:#555;">${event.locationName}</p>
            <a href="/events/${event.id}" style="font-weight:bold; color:#007bff; text-decoration:none;">Подробнее</a>
          </div>
        `);

      marker.on('click', () => {
        this.selectEvent(event.id, false);
      });

      this.markerMap.set(event.id, marker);
    });
  }

  private adjustMapBounds(L: any, eventList: any[]): void {
    if (!this.map) return;

    const validCoords = eventList
      .filter(e => e.latitude !== null && e.longitude !== null && !isNaN(Number(e.latitude)) && !isNaN(Number(e.longitude)))
      .map(e => [Number(e.latitude), Number(e.longitude)] as [number, number]);

    if (validCoords.length > 0) {
      const bounds = L.latLngBounds(validCoords);
      this.map.fitBounds(bounds, { padding: [50, 50] });
    }
  }

  public getMarkerColor(dateStr: string): string {
    const now = new Date();
    const eventDate = new Date(dateStr);
    const diffTime = eventDate.getTime() - now.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    if (diffDays < 0) return '#6c757d'; 
    if (diffDays <= 1) return '#dc3545'; 
    if (diffDays <= 3) return '#ffc107'; 
    if (diffDays <= 7) return '#28a745'; 
    return '#007bff'; 
  }

  public selectEvent(eventId: string, panTo: boolean = true): void {
    this.selectedEventId.set(eventId);
    
    const element = document.getElementById(`event-card-${eventId}`);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    }

    if (panTo && this.map) {
      const marker = this.markerMap.get(eventId);
      if (marker) {
        this.map.setView(marker.getLatLng(), 15);
        marker.openPopup();
      }
    }
  }

  public sortEvents(criteria: 'date' | 'location'): void {
    this.currentSort.set(criteria);
    
    const sorted = [...this.filteredEvents()].sort((a, b) => {
        if (criteria === 'date') {
        return new Date(a.startDate).getTime() - new Date(b.startDate).getTime();
        } else {
        return a.locationName.localeCompare(b.locationName);
        }
    });

    this.filteredEvents.set(sorted);
  }
}