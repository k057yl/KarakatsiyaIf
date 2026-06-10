import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest, VerifyCodeRequest } from '../dtos/auth.dto';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = `${environment.apiUrl}/auth`;
  
  private currentUserSubject = new BehaviorSubject<AuthResponse | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor() {
    if (typeof window !== 'undefined') {
      const savedUser = localStorage.getItem('karakatsiya_user');
      if (savedUser) this.currentUserSubject.next(JSON.parse(savedUser));
    }
  }

  register(data: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, data);
  }

  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, data).pipe(
      tap(res => this.setSession(res))
    );
  }

  verifyCode(data: VerifyCodeRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/verify-code`, data);
  }

  private setSession(user: AuthResponse) {
    if (typeof window !== 'undefined') {
      localStorage.setItem('karakatsiya_user', JSON.stringify(user));
    }
    this.currentUserSubject.next(user);
  }

  logout() {
    if (typeof window !== 'undefined') {
      localStorage.removeItem('karakatsiya_user');
    }
    this.currentUserSubject.next(null);
    this.router.navigate(['/']);
  }

  public isSuperAdmin(): boolean {
    const user = this.currentUserSubject.value;
    return user?.role === 'SuperAdmin';
  }
}