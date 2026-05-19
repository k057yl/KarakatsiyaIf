import { Component, inject, OnInit, signal, OnDestroy, PLATFORM_ID, afterNextRender, ElementRef, viewChild, effect, Injector, runInInjectionContext } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { EventService } from '../../../core/services/event.service';
import { TranslateModule } from '@ngx-translate/core';
import type * as LType from 'leaflet';

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
  private readonly injector = inject(Injector);

  private mapContainer = viewChild<ElementRef<HTMLDivElement>>('mapContainer');

  public eventDetails = signal<any>(null);
  public isLoading = signal<boolean>(true);
  public errorMessage = signal<string>('');

  private map: LType.Map | undefined;

  constructor() {
    afterNextRender(() => {
      runInInjectionContext(this.injector, () => {
        effect(async () => {
          const data = this.eventDetails();
          const container = this.mapContainer()?.nativeElement;

          if (data && data.latitude && data.longitude && container && !this.map) {
            await this.initMap(container, data.latitude, data.longitude);
          }
        });
      });
    });
  }

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
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'ERRORS.SERVICE_UNAVAILABLE');
        this.isLoading.set(false);
      }
    });
  }

  private async initMap(container: HTMLDivElement, lat: number, lng: number): Promise<void> {
    const L = await import('leaflet');

    this.map = L.map(container, {
      center: [lat, lng],
      zoom: 16,
      dragging: true,
      scrollWheelZoom: false
    });

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors'
    }).addTo(this.map);

    const customIcon = L.icon({
      iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
      iconRetinaUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png',
      shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
      iconSize: [25, 41],
      iconAnchor: [12, 41]
    });

    L.marker([lat, lng], { icon: customIcon }).addTo(this.map);

    setTimeout(() => {
      this.map?.invalidateSize();
    }, 50);
  }
}