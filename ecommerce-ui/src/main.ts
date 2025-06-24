import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { TokenInterceptor } from './app/services/token.interceptor';
import { AuthService } from './app/services/auth.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

bootstrapApplication(AppComponent, {
...appConfig,
  providers: [
    ...(appConfig.providers || []),
    provideHttpClient(),
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true },
   // AuthService // Optional if providedIn: 'root'
  ]
}).catch(err => console.error(err));
