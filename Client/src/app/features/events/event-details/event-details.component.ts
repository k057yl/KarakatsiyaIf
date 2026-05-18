import { Component, inject, OnInit, signal, OnDestroy, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { EventService } from '../../../core/services/event.service';
import { TranslateModule } from '@ngx-translate/core';
import * as L from 'leaflet';

@Component({
  selector: 'app-event-details',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  templateUrl: './event-details.component.html',
  styleUrls: ['./event-details.component.scss']
})
export class EventDetailsComponent implements OnInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly eventService = inject(EventService);
  private readonly platformId = inject(PLATFORM_ID);

  public eventDetails = signal<any>(null);
  public isLoading = signal<boolean>(true);
  public errorMessage = signal<string>('');

  private map: L.Map | undefined;

  public ngOnInit(): void {
    const eventId = this.route.snapshot.paramMap.get('id');
    
    if (eventId) {
      this.loadEventDetails(eventId);
    } else {
      this.errorMessage.set('ERRORS.EVENT_NOT_FOUND');
      this.isLoading.set(false);
    }
  }

  public ngOnDestroy(): void {
    if (this.map) {
      this.map.remove();
    }
  }

  private loadEventDetails(id: string): void {
    this.eventService.getEventDetails(id).subscribe({
      next: (data) => {
        this.eventDetails.set(data);
        this.isLoading.set(false);
        
        if (isPlatformBrowser(this.platformId) && data.latitude && data.longitude) {
          setTimeout(() => this.initMap(data.latitude, data.longitude), 100);
        }
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'ERRORS.SERVICE_UNAVAILABLE');
        this.isLoading.set(false);
      }
    });
  }

  private initMap(lat: number, lng: number): void {
    this.map = L.map('event-map', {
      center: [lat, lng],
      zoom: 16,
      dragging: false,
      scrollWheelZoom: false
    });

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors'
    }).addTo(this.map);

    const customIcon = L.icon({
      iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
      iconSize: [25, 41],
      iconAnchor: [12, 41]
    });

    L.marker([lat, lng], { icon: customIcon }).addTo(this.map);
  }
}