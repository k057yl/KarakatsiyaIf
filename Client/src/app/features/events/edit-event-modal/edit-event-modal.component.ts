import { Component, EventEmitter, inject, Output, signal, input, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { EventService } from '../services/event.service';
import { EventFormComponent } from '../../../shared/ui/event-form/event-form.component';
import { CreateEventPhotoDto } from '../dtos/create-event-photo.dto';

@Component({
  selector: 'app-edit-event-modal',
  standalone: true,
  imports: [CommonModule, TranslateModule, EventFormComponent],
  templateUrl: './edit-event-modal.component.html',
  styleUrls: ['./edit-event-modal.component.scss']
})
export class EditEventModalComponent implements OnInit {
  @Output() closeModal = new EventEmitter<void>();
  @Output() eventUpdated = new EventEmitter<string>();

  @ViewChild(EventFormComponent) innerForm!: EventFormComponent;

  public editEventData = input.required<any>();

  private eventService = inject(EventService);

  public isSubmitting = signal(false);
  public isUploadingPhoto = signal(false);
  public categoriesList = signal<any[]>([]);

  ngOnInit() {
    this.eventService.getCategories().subscribe({
      next: (cats) => this.categoriesList.set(cats),
      error: (err) => console.error('Не удалось загрузить категории для селекта', err)
    });
  }

  public onEditSubmit(eventData: { basePayload: any }) {
    this.isSubmitting.set(true);
    const updatePayload = { ...eventData.basePayload, photos: [] };
    const id = this.editEventData().id;

    this.eventService.updateEvent(id, updatePayload).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.eventUpdated.emit(id);
        this.close();
      },
      error: (err) => { 
        this.isSubmitting.set(false); 
        console.error('Ошибка апдейта:', err); 
      }
    });
  }

  public onUploadPhotoOnFly(data: { file: File, isMain: boolean }) {
  this.isUploadingPhoto.set(true);
  const id = this.editEventData().id;

  this.eventService.uploadPhoto(id, data.file, data.isMain).subscribe({
    next: (res: any) => {
      this.isUploadingPhoto.set(false);
      if (this.innerForm) {
        this.innerForm.uploadedPhotos.update((list: CreateEventPhotoDto[]) => 
          list.map((p: CreateEventPhotoDto) => 
            p.imageUrl.startsWith('data:') ? { ...p, imageUrl: res.url, publicId: res.publicId } : p
          )
        );
      }
    },
    error: () => this.isUploadingPhoto.set(false)
  });
}

  public close() {
    if (this.innerForm) {
      this.innerForm.clearFormAndStorage();
    }
    this.isSubmitting.set(false);
    this.isUploadingPhoto.set(false);
    this.closeModal.emit();
  }
}