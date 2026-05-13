import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { PendingOrganizer } from '../models/dtos/admin.dto';


@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/admin`;

  getPendingOrganizers(): Observable<PendingOrganizer[]> {
    return this.http.get<PendingOrganizer[]>(`${this.apiUrl}/organizers/pending`);
  }

  approveOrganizer(id: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/organizers/${id}/approve`, {});
  }
}