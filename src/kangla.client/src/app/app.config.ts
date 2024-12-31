import { ApplicationConfig, ErrorHandler, provideZoneChangeDetection, isDevMode } from '@angular/core';
import { provideRouter } from '@angular/router';
import { TokenInterceptor } from './core/auth/auth.interceptor';
import { HTTP_INTERCEPTORS, withInterceptorsFromDi } from '@angular/common/http';
import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient } from '@angular/common/http';
import { MAT_DIALOG_DEFAULT_OPTIONS } from '@angular/material/dialog';
import { GlobalErrorHandler } from './core/errors/global-error-handler';
import { LoadingInterceptor } from './core/loading/loading.interceptor';
import { provideServiceWorker } from '@angular/service-worker';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }), 
    provideRouter(routes), 
    provideAnimationsAsync(),
    provideHttpClient(withInterceptorsFromDi()),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoadingInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true,
    },
    {
      provide: ErrorHandler, 
      useClass: GlobalErrorHandler
    },
    {
      provide: MAT_DIALOG_DEFAULT_OPTIONS,
      useValue: { width: '400px' }
    }, provideServiceWorker('ngsw-worker.js', {
            enabled: !isDevMode(),
            registrationStrategy: 'registerWhenStable:30000'
          }) 
  ]
};
