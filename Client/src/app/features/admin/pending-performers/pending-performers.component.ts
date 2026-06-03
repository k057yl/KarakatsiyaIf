import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { environment } from '../../../../environments/environment';

interface PendingPerformer {
  id: string;
  name: string;
  slug: string;
}

@Component({
  selector: 'app-admin-performers',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './pending-performers.component.html',
  styleUrls: ['./pending-performers.component.scss']
})
export class AdminPerformersComponent implements OnInit {
  private http = inject(HttpClient);
  private fb = inject(FormBuilder);
  private apiUrl = `${environment.apiUrl}/admin/performers`;

  public performers = signal<PendingPerformer[]>([]);
  public isLoading = signal<boolean>(false);
  
  public selectedPerformer = signal<PendingPerformer | null>(null);
  public isMergeMode = signal<boolean>(false);

  verifyForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', Validators.maxLength(2000)],
    avatarUrl: ['', Validators.maxLength(500)],
    instagramUrl: [''],
    telegramUrl: [''],
    youtubeUrl: ['']
  });

  mergeForm = this.fb.nonNullable.group({
    targetId: ['', [Validators.required, Validators.pattern(/^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/)]]
  });

  ngOnInit() {
    this.loadPendingPerformers();
  }

  loadPendingPerformers() {
    this.isLoading.set(true);
    this.http.get<PendingPerformer[]>(`${this.apiUrl}/pending`).subscribe({
      next: (data) => {
        this.performers.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Ошибка загрузки артистов', err);
        this.isLoading.set(false);
      }
    });
  }

  openVerifyModal(performer: PendingPerformer) {
    this.isMergeMode.set(false);
    this.selectedPerformer.set(performer);
    this.verifyForm.patchValue({
      name: performer.name,
      description: '',
      avatarUrl: '',
      instagramUrl: '',
      telegramUrl: '',
      youtubeUrl: ''
    });
  }

  openMergeModal(performer: PendingPerformer) {
    this.isMergeMode.set(true);
    this.selectedPerformer.set(performer);
    this.mergeForm.reset();
  }

  closeModal() {
    this.selectedPerformer.set(null);
  }

  // Сабмит статуса "ЗАЕБИСЬ"
  submitVerify() {
    if (this.verifyForm.invalid || !this.selectedPerformer()) return;

    const payload = this.verifyForm.getRawValue();
    const id = this.selectedPerformer()!.id;

    this.http.put(`${this.apiUrl}/${id}/verify`, payload).subscribe({
      next: () => {
        this.removePerformerFromList(id);
        this.closeModal();
      },
      error: (err) => alert(err.error?.message || 'Ошибка верификации')
    });
  }

  submitMerge() {
    if (this.mergeForm.invalid || !this.selectedPerformer()) return;

    const sourceId = this.selectedPerformer()!.id;
    const targetId = this.mergeForm.controls.targetId.value.trim();

    if (sourceId === targetId) {
      alert('Ты не можешь слить артиста самого в себя, гений.');
      return;
    }

    this.http.post(`${this.apiUrl}/${sourceId}/merge-into/${targetId}`, {}).subscribe({
      next: () => {
        this.removePerformerFromList(sourceId);
        this.closeModal();
      },
      error: (err) => alert(err.error?.message || 'Ошибка слияния')
    });
  }

  private removePerformerFromList(id: string) {
    this.performers.update(list => list.filter(p => p.id !== id));
  }
}