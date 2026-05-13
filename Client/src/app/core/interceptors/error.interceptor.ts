import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError(err => {
      if (err.status === 401) {
        authService.logout();
      }
      
      const error = err.error?.message || err.statusText;
      return throwError(() => error);
    })
  );
};