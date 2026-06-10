import { mergeApplicationConfig, ApplicationConfig } from '@angular/core';
import { provideServerRendering } from '@angular/platform-server';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { appConfig } from './app.config';
import { serverUrlInterceptor } from './core/interceptors/server-url.interceptor';

const serverConfig: ApplicationConfig = {
  providers: [
    provideServerRendering(),
    provideHttpClient(
      withInterceptors([serverUrlInterceptor])
    )
  ]
};

export const config = mergeApplicationConfig(appConfig, serverConfig);