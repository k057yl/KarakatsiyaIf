import { Component, EventEmitter, inject, Output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

export interface PerformerCreatedEvent {
  id: string;
  name: string;
}

@Component({
  selector: 'app-create-performer-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-performer-modal.component.html',
  styleUrls: ['./create-performer-modal.component.scss']
})
export class CreatePerformerModalComponent {
  @Output() closeModal = new EventEmitter<void>();
  @Output() performerCreated = new EventEmitter<PerformerCreatedEvent>();

  private fb = inject(FormBuilder);
  private http = inject(HttpClient);

  public isCreating = signal<boolean>(false);

  performerForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]]
  });

  public submit() {
    if (this.performerForm.invalid) return;
    
    const name = this.performerForm.controls.name.value.trim();
    this.isCreating.set(true);

    this.http.post<{ id: string }>(`${environment.apiUrl}/performers`, { name }).subscribe({
      next: (res: { id: string }) => {
        this.isCreating.set(false);
        this.performerCreated.emit({ id: res.id, name });
      },
      error: (err: HttpErrorResponse) => {
        this.isCreating.set(false);
        alert(err.error?.message || 'Ошибка создания артиста. Возможно, он уже существует.');
      }
    });
  }

  public close() {
    if (!this.isCreating()) {
      this.closeModal.emit();
    }
  }
}