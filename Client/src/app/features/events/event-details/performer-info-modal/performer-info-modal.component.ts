import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { PerformerDetails } from '../../dtos/performer-details.dto';
import { ASSET_CONSTANTS } from '../../../../core/constants/asset-constants';

@Component({
  selector: 'app-performer-info-modal',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './performer-info-modal.component.html',
  styleUrls: ['./performer-info-modal.component.scss']
})
export class PerformerInfoModalComponent {
  @Input({ required: true }) performer!: PerformerDetails;
  @Output() closeModal = new EventEmitter<void>();

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

  public close(): void {
    this.closeModal.emit();
  }
}