import { Component, EventEmitter, inject, Output, signal, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { EventService } from '../services/event.service';
import { PaymentService } from '../../../shared/services/payment.service';
import { EventFormComponent } from '../../../shared/ui/event-form/event-form.component';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-create-event-modal',
  standalone: true,
  imports: [CommonModule, TranslateModule, EventFormComponent],
  templateUrl: './create-event-modal.component.html',
  styleUrls: ['./create-event-modal.component.scss']
})
export class CreateEventModalComponent implements OnInit {
  @Output() closeModal = new EventEmitter<void>();
  @Output() eventCreated = new EventEmitter<string>();

  @ViewChild(EventFormComponent) innerForm!: EventFormComponent;

  private eventService = inject(EventService);
  private paymentService = inject(PaymentService);

  public isSubmitting = signal(false);
  public isUploadingPhoto = signal(false);
  public isSuccessScreen = signal(false);
  public createdEventId = signal<string | null>(null);
  public categoriesList = signal<any[]>([]);

  ngOnInit() {
    this.eventService.getCategories().subscribe({
      next: (cats) => this.categoriesList.set(cats),
      error: (err) => console.error('Не удалось загрузить категории для селекта', err)
    });
  }

  public async onCreateSubmit(eventData: { basePayload: any, selectedFiles: { file: File, isMain: boolean }[] }) {
    this.isSubmitting.set(true);

    const createPayload = { 
      ...eventData.basePayload, 
      photos: []
    };

    this.eventService.createEvent(createPayload).subscribe({
      next: async (res) => {
        this.createdEventId.set(res.eventId);
        localStorage.removeItem('event_draft_form');

        if (eventData.selectedFiles.length > 0) {
          this.isUploadingPhoto.set(true);
          
          try {
            for (const item of eventData.selectedFiles) {
              await this.eventService.uploadPhoto(res.eventId, item.file, item.isMain).toPromise();
            }

            this.isUploadingPhoto.set(false);
            this.isSubmitting.set(false);
            this.isSuccessScreen.set(true);

          } catch (err) {
            this.isUploadingPhoto.set(false);
            this.isSubmitting.set(false);
            console.error('Ошибка поочередной загрузки медиа:', err);
            alert('Не удалось загрузить одну или несколько фотографий. Проверь размер файлов.');
          }

        } else {
          this.isSubmitting.set(false);
          this.isSuccessScreen.set(true);
        }
      },
      error: (err) => { 
        this.isSubmitting.set(false); 
        console.error('Ошибка создания события:', err); 
      }
    });
  }

  public close() {
    if (this.innerForm) {
      this.innerForm.clearFormAndStorage();
    }
    this.isSuccessScreen.set(false);
    this.isSubmitting.set(false);
    this.isUploadingPhoto.set(false);
    this.createdEventId.set(null);
    this.closeModal.emit();
  }

  public finishWithoutVip() {
    if (this.createdEventId()) {
      this.eventCreated.emit(this.createdEventId()!);
    }
    this.close();
  }

  public buyVip() {
    const id = this.createdEventId();
    if (id) {
      this.paymentService.payForVip(id);
    }
  }
}