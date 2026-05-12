import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BehaviorSubject, Observable, tap } from 'rxjs';

export interface AuthResponse {
  token: string;
  email: string;
  role: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/auth`;
  
  private currentUserSubject = new BehaviorSubject<AuthResponse | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor() {
    const savedUser = localStorage.getItem('karakatsiya_user');
    if (savedUser) this.currentUserSubject.next(JSON.parse(savedUser));
  }

  register(email: string, password?: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, { email, password }); 
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, { email, password }).pipe(
      tap(res => this.setSession(res))
    );
  }

  verifyCode(email: string, code: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/verify-code`, { email, code }).pipe(
      tap(res => this.setSession(res))
    );
  }

  private setSession(user: AuthResponse) {
    localStorage.setItem('karakatsiya_user', JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  logout() {
    localStorage.removeItem('karakatsiya_user');
    this.currentUserSubject.next(null);
  }
}