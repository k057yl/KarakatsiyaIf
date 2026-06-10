import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { AuthService } from '../../features/auth/services/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {

      if (error.status === 401) {
        console.warn('[Error Interceptor] Словлен 401. Вычищаем сессию...');

        if (typeof window !== 'undefined') {
          authService.logout();
        }
      }

      if (error.status === 0) {
        console.error('[Error Interceptor] Сетевая ошибка или сервер недоступен (Status 0). Не логаутим.');
      }

      return throwError(() => error);
    })
  );
};