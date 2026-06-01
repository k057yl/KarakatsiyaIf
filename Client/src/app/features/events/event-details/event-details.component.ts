import { Component, inject, OnInit, OnDestroy, ElementRef, viewChild, effect, Injector, runInInjectionContext, afterNextRender, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { EventService } from '../services/event.service';
import { CommentService } from '../../../core/services/comment.service';
import { AuthService } from '../../../core/services/auth.service';
import { TranslateModule } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-event-details',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule, FormsModule],
  templateUrl: './event-details.component.html',
  styleUrls: ['./event-details.component.scss']
})
export class EventDetailsComponent implements OnInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly eventService = inject(EventService);
  private readonly commentService = inject(CommentService);
  public readonly authService = inject(AuthService);
  private readonly injector = inject(Injector);

  private mapContainer = viewChild<ElementRef<HTMLDivElement>>('mapContainer');

  public eventDetails = signal<any>(null);
  public isLoading = signal<boolean>(true);
  public errorMessage = signal<string>('');

  public commentText = signal<string>('');
  public showInst = signal<boolean>(false);
  public showTg = signal<boolean>(false);
  public isCommentSubmitting = signal<boolean>(false);

  private map: any;

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
        if (data && data.photos && data.photos.length > 0) {
          const main = data.photos.find((p: any) => p.isMain);
          data.mainPhotoUrl = main ? main.imageUrl : data.photos[0].imageUrl;
        }
        this.eventDetails.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'ERRORS.SERVICE_UNAVAILABLE');
        this.isLoading.set(false);
      }
    });
  }

  public canLeaveComment(startDate: string): boolean {
    if (!startDate) return false;
    
    const eventTime = new Date(startDate).getTime();
    const now = new Date().getTime();
    const oneHourOffset = 60 * 60 * 1000; 
    
    return now >= (eventTime + oneHourOffset);
  }

  public sendComment(): void {
    const text = this.commentText().trim();
    const event = this.eventDetails();
    if (!text || !event) return;

    this.isCommentSubmitting.set(true);

    this.commentService.createComment({
      eventId: event.id,
      text: text,
      showInstagram: this.showInst(),
      showTelegram: this.showTg()
    }).subscribe({
      next: () => {
        this.loadEventDetails(event.id);
        this.commentText.set(''); 
        this.isCommentSubmitting.set(false);
      },
      error: () => {
        this.isCommentSubmitting.set(false);
      }
    });
  }

  public reportComment(commentId: string): void {
    const reason = window.prompt('Какая причина жалобы? (Спам, мат, оскорбления, реклама крипты):');
    if (!reason || !reason.trim()) return;

    this.commentService.reportComment(commentId, reason.trim()).subscribe({
      next: () => {
        window.alert('Жалоба отправлена на стол Суперадмину. Разберёмся!');
      },
      error: (err) => {
        window.alert(err.error?.message || 'Не удалось отправить жалобу.');
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