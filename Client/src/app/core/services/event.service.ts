import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { CreateEventDto } from '../models/dtos/event.dto';

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/events`;

  createEvent(payload: CreateEventDto): Observable<{ message: string, eventId: string }> {
    return this.http.post<{ message: string, eventId: string }>(this.apiUrl, payload);
  }

  getApprovedEvents(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl); 
  }

  getEventDetails(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  getArchivedEvents(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/archive`);
  }

  public getOrganizerEvents(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my`);
  }

  public updateEvent(id: string, payload: any): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, payload);
  }
}