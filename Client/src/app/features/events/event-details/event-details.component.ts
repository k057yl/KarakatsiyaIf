import { Component, inject, OnInit, OnDestroy, ElementRef, viewChild, effect, Injector, runInInjectionContext, afterNextRender, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { EventService } from '../services/event.service';
import { CommentService } from '../services/comment.service';
import { AuthService } from '../../auth/services/auth.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';
import { PerformerInfoModalComponent } from './performer-info-modal/performer-info-modal.component';
import { PerformerDetails } from '../dtos/performer-details.dto';
import { ASSET_CONSTANTS } from '../../../core/constants/asset-constants'; 

@Component({
  selector: 'app-event-details',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule, FormsModule, PerformerInfoModalComponent],
  templateUrl: './event-details.component.html',
  styleUrls: ['./event-details.component.scss']
})
export class EventDetailsComponent implements OnInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly eventService = inject(EventService);
  private readonly commentService = inject(CommentService);
  public readonly authService = inject(AuthService);
  private readonly translateService = inject(TranslateService);
  private readonly injector = inject(Injector);

  private mapContainer = viewChild<ElementRef<HTMLDivElement>>('mapContainer');

  public eventDetails = signal<any>(null);

  public currentPhotoUrl = signal<string | null>(null);

  public isLoading = signal<boolean>(true);
  public errorMessage = signal<string>('');

  public commentText = signal<string>('');
  public showInst = signal<boolean>(false);
  public showTg = signal<boolean>(false);
  public isCommentSubmitting = signal<boolean>(false);
  public activePerformer = signal<PerformerDetails | null>(null);

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

  public getAvatar(avatarUrl: string | null, performerId: string): string {
    return ASSET_CONSTANTS.getPerformerAvatar(avatarUrl, performerId);
  }

  private loadEventDetails(id: string): void {
    this.eventService.getEventDetails(id).subscribe({
      next: (data) => {
        let mainPhoto = '';
        if (data && data.photos && data.photos.length > 0) {
          const main = data.photos.find((p: any) => p.isMain);
          mainPhoto = main ? main.imageUrl : data.photos[0].imageUrl;
          data.mainPhotoUrl = mainPhoto;
        }
        
        this.eventDetails.set(data);
        
        if (mainPhoto) {
          this.currentPhotoUrl.set(mainPhoto);
        }
        
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message);
        this.isLoading.set(false);
      }
    });
  }
  
  public changePhoto(photoUrl: string): void {
    this.currentPhotoUrl.set(photoUrl);
  }

  public canLeaveComment(startDate: string | null | undefined): boolean {
    if (!startDate) return false;

    const now = new Date();
    const eventTime = new Date(startDate);
    const diffMs = now.getTime() - eventTime.getTime();
    const diffHours = diffMs / (1000 * 60 * 60);

    return diffHours >= 1;
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
    const promptMsg = this.translateService.instant('EVENT_DETAILS.REPORT_PROMPT');
    const reason = window.prompt(promptMsg);
    
    if (!reason || !reason.trim()) return;

    this.commentService.reportComment(commentId, reason.trim()).subscribe({
      next: () => {
        const successMsg = this.translateService.instant('EVENT_DETAILS.REPORT_SUCCESS');
        window.alert(successMsg);
      },
      error: (err) => {
        const fallbackError = this.translateService.instant('ERRORS.SERVICE_UNAVAILABLE');
        const errorKey = err.error?.message;
        const errorMsg = errorKey ? this.translateService.instant(errorKey) : fallbackError;
        window.alert(errorMsg);
      }
    });
  }

  public openPerformerInfo(performer: PerformerDetails): void {
    this.activePerformer.set(performer);
  }

  public closePerformerInfo(): void {
    this.activePerformer.set(null);
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