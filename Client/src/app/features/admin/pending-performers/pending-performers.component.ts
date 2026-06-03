import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { environment } from '../../../../environments/environment';

interface AdminPerformer {
  id: string;
  name: string;
  slug: string;
  isVerified: boolean;
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

  public performers = signal<AdminPerformer[]>([]);
  public isLoading = signal<boolean>(false);
  public currentTab = signal<'pending' | 'all'>('pending');
  public searchQuery = signal<string>('');
  
  public selectedPerformer = signal<AdminPerformer | null>(null);
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
    this.loadPerformers();
  }

  public switchTab(tab: 'pending' | 'all') {
    this.currentTab.set(tab);
    this.searchQuery.set('');
    this.loadPerformers();
  }

  public loadPerformers() {
    this.isLoading.set(true);
    
    const endpoint = this.currentTab() === 'pending' 
      ? `${this.apiUrl}/pending` 
      : `${this.apiUrl}?search=${this.searchQuery()}`;

    this.http.get<AdminPerformer[]>(endpoint).subscribe({
      next: (data) => {
        this.performers.set(data);
        this.isLoading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        console.error('Ошибка загрузки артистов', err);
        this.isLoading.set(false);
      }
    });
  }

  public onSearch(event: Event) {
    const target = event.target as HTMLInputElement;
    this.searchQuery.set(target.value);
    this.loadPerformers();
  }

  public openVerifyModal(performer: AdminPerformer) {
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

  public openMergeModal(performer: AdminPerformer) {
    this.isMergeMode.set(true);
    this.selectedPerformer.set(performer);
    this.mergeForm.reset();
  }

  public closeModal() {
    this.selectedPerformer.set(null);
  }

  public submitVerify() {
    if (this.verifyForm.invalid || !this.selectedPerformer()) return;

    const payload = this.verifyForm.getRawValue();
    const id = this.selectedPerformer()!.id;

    this.http.put(`${this.apiUrl}/${id}/verify`, payload).subscribe({
      next: () => {
        this.loadPerformers(); 
        this.closeModal();
      },
      error: (err: HttpErrorResponse) => alert(err.error?.message || 'Ошибка верификации')
    });
  }

  public submitMerge() {
    if (this.mergeForm.invalid || !this.selectedPerformer()) return;

    const sourceId = this.selectedPerformer()!.id;
    const targetId = this.mergeForm.controls.targetId.value.trim();

    this.http.post(`${this.apiUrl}/${sourceId}/merge-into/${targetId}`, {}).subscribe({
      next: () => {
        this.performers.update(list => list.filter(p => p.id !== sourceId));
        this.closeModal();
      },
      error: (err: HttpErrorResponse) => alert(err.error?.message || 'Ошибка слияния')
    });
  }

  public deletePerformer(id: string) {
    if (!confirm('Ты уверен, что хочешь насовсем удалить этого артиста? Все его связи с концертами сотрутся.')) return;

    this.http.delete(`${this.apiUrl}/${id}`).subscribe({
      next: () => {
        this.performers.update(list => list.filter(p => p.id !== id));
      },
      error: (err: HttpErrorResponse) => alert(err.error?.message || 'Ошибка удаления')
    });
  }
}