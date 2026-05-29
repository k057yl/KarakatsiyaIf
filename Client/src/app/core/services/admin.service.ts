import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http'; 
import { environment } from '../../../environments/environment';
import { PendingOrganizer } from '../models/dtos/admin.dto';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);
  
  // --- ОРГАНИЗАТОРЫ ---
  private orgApiUrl = `${environment.apiUrl}/admin/organizers`;

  getPendingOrganizers(): Observable<PendingOrganizer[]> {
    return this.http.get<PendingOrganizer[]>(`${this.orgApiUrl}/pending`);
  }
  approveOrganizer(id: string): Observable<any> {
    return this.http.post(`${this.orgApiUrl}/${id}/approve`, {});
  }
  rejectOrganizer(id: string, reason: string): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post(`${this.orgApiUrl}/${id}/reject`, JSON.stringify(reason), { headers });
  }

  // --- ИВЕНТЫ ---
  private eventsApiUrl = `${environment.apiUrl}/admin/events`;

  getPendingEvents(): Observable<any[]> {
    return this.http.get<any[]>(`${this.eventsApiUrl}/pending`);
  }
  approveEvent(id: string, isVip: boolean): Observable<any> {
    return this.http.post(`${this.eventsApiUrl}/${id}/approve?isVip=${isVip}`, {});
  }
  rejectEvent(id: string, reason: string): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post(`${this.eventsApiUrl}/${id}/reject`, JSON.stringify(reason), { headers });
  }
  deleteEvent(id: string): Observable<any> {
    return this.http.delete(`${this.eventsApiUrl}/${id}`);
  }
  sendToFix(id: string, reason: string): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post(`${this.eventsApiUrl}/${id}/fix`, JSON.stringify(reason), { headers });
  }
  toggleVip(id: string): Observable<any> {
    return this.http.post(`${this.eventsApiUrl}/${id}/toggle-vip`, {});
  }
  getActiveEvents(): Observable<any[]> {
    return this.http.get<any[]>(`${this.eventsApiUrl}/active`);
  }

  // --- МОДЕРАЦИЯ КОММЕНТАРИЕВ ---
  private commentsAdminUrl = `${environment.apiUrl}/admin/comments`;

  getReportedComments(): Observable<any[]> {
    return this.http.get<any[]>(`${this.commentsAdminUrl}/reported`);
  }

  deleteCommentByReport(commentId: string): Observable<any> {
    return this.http.delete(`${this.commentsAdminUrl}/${commentId}/confirm-report`);
  }

  dismissReport(commentId: string): Observable<any> {
    return this.http.post(`${this.commentsAdminUrl}/${commentId}/dismiss-report`, {});
  }
}