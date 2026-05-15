import { Component, EventEmitter, inject, Output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { EventService } from '../../../core/services/event.service';

@Component({
  selector: 'app-create-event-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-event-modal.component.html',
  styleUrls: ['./create-event-modal.component.scss']
})
export class CreateEventModalComponent {
  @Output() closeModal = new EventEmitter<void>();
  @Output() eventCreated = new EventEmitter<string>();

  private fb = inject(FormBuilder);
  private eventService = inject(EventService);

  isSubmitting = signal(false);
  errorMsg = signal('');

  eventForm = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(300)]],
    description: ['', Validators.required],
    startDate: ['', Validators.required],
    locationName: ['', Validators.required],
    city: ['', Validators.required],
    street: ['', Validators.required],
    houseNumber: ['']
  });

  close() {
    this.closeModal.emit();
  }

  submit() {
    if (this.eventForm.invalid) return;

    this.isSubmitting.set(true);
    this.errorMsg.set('');

    const formValue = this.eventForm.getRawValue();
    
    const payload = {
      ...formValue,
      startDate: new Date(formValue.startDate).toISOString()
    };

    this.eventService.createEvent(payload).subscribe({
      next: (res) => {
        this.isSubmitting.set(false);
        this.eventCreated.emit(res.eventId);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMsg.set(err.error?.message || 'Пиздец, не удалось создать ивент.');
      }
    });
  }
}