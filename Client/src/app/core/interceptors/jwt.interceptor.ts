import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../../features/auth/services/auth.service';
import { environment } from '../../../environments/environment';
import { take } from 'rxjs';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.url.includes('.json') || req.url.includes('/assets/i18n/')) {
    return next(req);
  }

  const authService = inject(AuthService);
  const apiUrl = environment.apiUrl;

  let token: string | null = null;

  authService.currentUser$.pipe(take(1)).subscribe(user => {
    token = user?.token || null;
  });

  const isApiUrl = req.url.startsWith(apiUrl);

  if (token && isApiUrl) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req);
};