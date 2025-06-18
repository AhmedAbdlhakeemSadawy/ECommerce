import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { TokenInterceptor } from './services/token.interceptor';
import { provideHttpClient,withInterceptorsFromDi } from '@angular/common/http';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),

        // ✅ Register the interceptor in DI
        {
          provide: TokenInterceptor,
          useClass: TokenInterceptor
        }
  ]
};
