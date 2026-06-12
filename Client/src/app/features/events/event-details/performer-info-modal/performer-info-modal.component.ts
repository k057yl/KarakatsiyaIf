import { Component, EventEmitter, Input, Output, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { PerformerDetails } from '../../dtos/performer-details.dto';
import { EventService } from '../../../events/services/event.service';
import { ASSET_CONSTANTS } from '../../../../core/constants/asset-constants';

@Component({
  selector: 'app-performer-info-modal',
  standalone: true,
  imports: [CommonModule, TranslateModule, RouterLink],
  templateUrl: './performer-info-modal.component.html',
  styleUrls: ['./performer-info-modal.component.scss']})
export class PerformerInfoModalComponent implements OnInit {
  @Input({ required: true }) performer!: PerformerDetails;
  @Output() closeModal = new EventEmitter<void>();

  private readonly eventService = inject(EventService);

  public performerEvents = signal<any[]>([]);
  public isLoadingEvents = signal<boolean>(true);

  public get avatarSrc(): string {
    return ASSET_CONSTANTS.getPerformerAvatar(this.performer.avatarUrl, this.performer.id);
  }

  public get instagramLink(): string {
    if (!this.performer.instagramUrl) return '';
    return this.performer.instagramUrl.startsWith('http')
      ? this.performer.instagramUrl
      : `https://instagram.com/${this.performer.instagramUrl.replace(/^@/, '')}`;
  }

  public get telegramLink(): string {
    if (!this.performer.telegramUrl) return '';
    return this.performer.telegramUrl.startsWith('http')
      ? this.performer.telegramUrl
      : `https://t.me/${this.performer.telegramUrl.replace(/^@/, '')}`;
  }

  public get youtubeLink(): string {
    if (!this.performer.youtubeUrl) return '';
    return this.performer.youtubeUrl.startsWith('http')
      ? this.performer.youtubeUrl
      : `https://youtube.com/${this.performer.youtubeUrl}`;
  }

  public ngOnInit(): void {
    this.loadPerformerEvents();
  }

  private loadPerformerEvents(): void {
    this.eventService.getApprovedEvents().subscribe({
      next: (events) => {
        const now = new Date();
        const filtered = events.filter((e: any) => {
          const hasPerformer = (e.performers || e.Performers || [])
            .some((p: any) => p.id === this.performer.id);
          const isFuture = new Date(e.startDate).getTime() > now.getTime();
          return hasPerformer && isFuture;
        });
        
        this.performerEvents.set(filtered);
        this.isLoadingEvents.set(false);
      },
      error: () => {
        this.isLoadingEvents.set(false);
      }
    });
  }

  public close(): void {
    this.closeModal.emit();
  }
}