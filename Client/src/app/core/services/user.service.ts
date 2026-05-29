import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UpdateContactsDto } from '../models/dtos/user.dto';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/users`;

  public updateContacts(data: UpdateContactsDto): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/me/contacts`, data);
  }

  public getMyProfile(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/me`);
  }

  public generateTelegramOtp(): Observable<{ code: string }> {
    return this.http.post<{ code: string }>(`${this.apiUrl}/me/telegram/generate-otp`, {});
  }

  public unlinkTelegram(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/me/telegram/unlink`, {});
  }
}