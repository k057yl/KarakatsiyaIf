import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { PerformerDetails } from '../../dtos/performer-details.dto';

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

  public close(): void {
    this.closeModal.emit();
  }
}