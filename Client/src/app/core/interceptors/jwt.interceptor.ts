import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { environment } from '../../../environments/environment';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const apiUrl = environment.apiUrl;

  let currentUser: any;
  authService.currentUser$.subscribe(user => currentUser = user).unsubscribe();

  const isLoggedIn = currentUser && currentUser.token;
  const isApiUrl = req.url.startsWith(apiUrl);

  if (isLoggedIn && isApiUrl) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${currentUser.token}`
      }
    });
  }

  return next(req);
};