import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { environment } from '../../../../environments/environment';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ASSET_CONSTANTS } from '../../../core/constants/asset-constants';

interface AdminPerformer {
  id: string;
  name: string;
  slug: string;
  isVerified: boolean;
  avatarUrl?: string;
  description?: string;
  instagramUrl?: string;
  telegramUrl?: string;
  youtubeUrl?: string;
}

@Component({
  selector: 'app-admin-performers',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './pending-performers.component.html',
  styleUrls: ['./pending-performers.component.scss']
})
export class AdminPerformersComponent implements OnInit {
  private http = inject(HttpClient);
  private fb = inject(FormBuilder);
  private apiUrl = `${environment.apiUrl}/admin/performers`;
  private translate = inject(TranslateService);

  public performers = signal<AdminPerformer[]>([]);
  public isLoading = signal<boolean>(false);
  public currentTab = signal<'pending' | 'all'>('pending');
  public searchQuery = signal<string>('');
  
  public selectedPerformer = signal<AdminPerformer | null>(null);
  public isMergeMode = signal<boolean>(false);
  public isUploadingFile = signal<boolean>(false);

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

  public get avatarPreview(): string {
    const formUrl = this.verifyForm.controls.avatarUrl.value;
    const currentId = this.selectedPerformer()?.id || '';
    return ASSET_CONSTANTS.getPerformerAvatar(formUrl, currentId);
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
      description: performer.description || '',
      avatarUrl: performer.avatarUrl || '',
      instagramUrl: performer.instagramUrl || '',
      telegramUrl: performer.telegramUrl || '',
      youtubeUrl: performer.youtubeUrl || ''
    });
  }

  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];
    this.isUploadingFile.set(true);

    this.compressImage(file, 200, 200, 0.8).then((compressedBlob) => {
      const formData = new FormData();
      formData.append('file', compressedBlob, 'avatar.webp');

      this.http.post<{ url: string }>(`${this.apiUrl}/upload-avatar`, formData).subscribe({
        next: (res) => {
          const avatarControl = this.verifyForm.controls.avatarUrl;
          avatarControl.setValue(res.url);
          avatarControl.markAsDirty();
          avatarControl.markAsTouched();
          avatarControl.updateValueAndValidity();

          this.isUploadingFile.set(false);
        },
        error: (err: HttpErrorResponse) => {
          console.error('Ошибка загрузки медиа', err);
          this.isUploadingFile.set(false);
          alert('Не удалось загрузить изображение');
        }
      });
    }).catch((err) => {
      console.error('Ошибка сжатия изображения', err);
      this.isUploadingFile.set(false);
    });
  }

  private compressImage(file: File, maxWidth: number, maxHeight: number, quality: number): Promise<Blob> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      
      reader.onload = (event: any) => {
        const img = new Image();
        img.src = event.target.result;
        
        img.onload = () => {
          const canvas = document.createElement('canvas');
          let width = img.width;
          let height = img.height;

          if (width > height) {
            if (width > maxWidth) {
              height = Math.round((height * maxWidth) / width);
              width = maxWidth;
            }
          } else {
            if (height > maxHeight) {
              width = Math.round((width * maxHeight) / height);
              height = maxHeight;
            }
          }

          canvas.width = width;
          canvas.height = height;

          const ctx = canvas.getContext('2d');
          if (!ctx) {
            reject(new Error('Canvas context is null'));
            return;
          }

          ctx.drawImage(img, 0, 0, width, height);

          canvas.toBlob((blob) => {
            if (blob) {
              resolve(blob);
            } else {
              reject(new Error('Canvas toBlob failed'));
            }
          }, 'image/webp', quality);
        };

        img.onerror = (err) => reject(err);
      };
      
      reader.onerror = (err) => reject(err);
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
    const confirmMsg = this.translate.instant('ADMIN_PERFORMERS.DELETE_CONFIRM');
    if (!confirm(confirmMsg)) return;

    this.http.delete(`${this.apiUrl}/${id}`).subscribe({
      next: () => {
        this.performers.update(list => list.filter(p => p.id !== id));
      },
      error: (err: HttpErrorResponse) => {
        const errMsg = err.error?.message || this.translate.instant('ERRORS.INTERNAL_SERVER_ERROR');
        alert(errMsg);
      }
    });
  }
}