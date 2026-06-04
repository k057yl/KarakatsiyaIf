import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { environment } from '../../../../environments/environment';

interface AdminOrganizer {
  id: string;
  name: string;
  email: string;
  phone?: string;
  website?: string;
  telegram?: string;
  instagram?: string;
  isApproved: boolean;
}

@Component({
  selector: 'app-admin-organizers',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './pending-organizers.component.html',
  styleUrls: ['./pending-organizers.component.scss']
})
export class PendingOrganizersComponent implements OnInit {
  private http = inject(HttpClient);
  private fb = inject(FormBuilder);
  private translate = inject(TranslateService);
  private apiUrl = `${environment.apiUrl}/admin/organizers`;

  public organizers = signal<AdminOrganizer[]>([]);
  public isLoading = signal<boolean>(false);
  public currentTab = signal<'requests' | 'all'>('requests');
  public searchQuery = signal<string>('');

  public selectedOrganizer = signal<AdminOrganizer | null>(null);

  editForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    email: ['', [Validators.required, Validators.email]],
    phone: [''],
    website: [''],
    telegram: [''],
    instagram: ['']
  });

  ngOnInit() {
    this.loadOrganizers();
  }

  public switchTab(tab: 'requests' | 'all') {
    this.currentTab.set(tab);
    this.searchQuery.set('');
    this.loadOrganizers();
  }

  public loadOrganizers() {
    this.isLoading.set(true);
    
    const endpoint = this.currentTab() === 'requests'
      ? `${this.apiUrl}/pending`
      : `${this.apiUrl}?search=${this.searchQuery()}`;

    this.http.get<AdminOrganizer[]>(endpoint).subscribe({
      next: (data) => {
        this.organizers.set(data);
        this.isLoading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this.isLoading.set(false);
      }
    });
  }

  public onSearch(event: Event) {
    const target = event.target as HTMLInputElement;
    this.searchQuery.set(target.value);
    this.loadOrganizers();
  }

  public approve(id: string) {
    this.http.post(`${this.apiUrl}/${id}/approve`, {}).subscribe({
      next: () => this.loadOrganizers(),
      error: (err: HttpErrorResponse) => alert(err.error?.message || 'Error')
    });
  }

  public openEditModal(org: AdminOrganizer) {
    this.selectedOrganizer.set(org);
    this.editForm.patchValue({
      name: org.name,
      email: org.email,
      phone: org.phone || '',
      website: org.website || '',
      telegram: org.telegram || '',
      instagram: org.instagram || ''
    });
  }

  public submitEdit() {
    if (this.editForm.invalid || !this.selectedOrganizer()) return;
    
    const id = this.selectedOrganizer()!.id;
    const payload = { id, ...this.editForm.getRawValue() };

    this.http.put(`${this.apiUrl}/${id}`, payload).subscribe({
      next: () => {
        this.loadOrganizers();
        this.selectedOrganizer.set(null);
      },
      error: (err: HttpErrorResponse) => alert(err.error?.message || 'Error')
    });
  }

  public deleteOrganizer(id: string) {
    const confirmMsg = this.translate.instant('ADMIN_ORGANIZERS.DELETE_CONFIRM');
    if (!confirm(confirmMsg)) return;

    this.http.delete(`${this.apiUrl}/${id}`).subscribe({
      next: () => {
        this.organizers.update(list => list.filter(o => o.id !== id));
      },
      error: (err: HttpErrorResponse) => alert(err.error?.message || 'Error')
    });
  }
}