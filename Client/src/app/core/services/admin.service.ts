import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http'; 
import { environment } from '../../../environments/environment';
import { PendingOrganizer } from '../models/dtos/admin.dto';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/admin/organizers`;

  getPendingOrganizers(): Observable<PendingOrganizer[]> {
    return this.http.get<PendingOrganizer[]>(`${this.apiUrl}/pending`);
  }

  approveOrganizer(id: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/approve`, {});
  }

  rejectOrganizer(id: string, reason: string): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post(`${this.apiUrl}/${id}/reject`, JSON.stringify(reason), { headers });
  }
}