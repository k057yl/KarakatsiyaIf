import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection, APP_INITIALIZER } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { provideClientHydration } from '@angular/platform-browser';

import { routes } from './app.routes';
import { jwtInterceptor } from './core/interceptors/jwt.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';

import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader, TRANSLATE_HTTP_LOADER_CONFIG } from '@ngx-translate/http-loader';
import { AuthService } from './features/auth/services/auth.service';
import { Observable, of, forkJoin } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

function HttpLoaderFactory(): TranslateHttpLoader {
  return new TranslateHttpLoader();
}

export function initAppFactory(translate: TranslateService, authService: AuthService) {
  return (): Observable<any> => {
    translate.addLangs(['uk', 'ru', 'en']);
    const defaultLang = 'uk';
    translate.setDefaultLang(defaultLang);

    return forkJoin({
      translate: translate.use(defaultLang)
    }).pipe(
      tap(() => console.log(`[App Init] Локализация и авторизация подготовлены.`)),
      catchError(err => {
        console.error('[App Init] Ошибка инициализации приложения:', err);
        return of(null);
      })
    );
  };
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideClientHydration(),
    provideHttpClient(
      withFetch(),
      withInterceptors([jwtInterceptor, errorInterceptor])
    ),
    importProvidersFrom(
      TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory
        },
        fallbackLang: 'uk'
      })
    ),
    {
      provide: TRANSLATE_HTTP_LOADER_CONFIG,
      useValue: {
        prefix: '/assets/i18n/',
        suffix: '.json'
      }
    },
    {
      provide: APP_INITIALIZER,
      useFactory: initAppFactory,
      deps: [TranslateService, AuthService],
      multi: true
    }
  ]
};