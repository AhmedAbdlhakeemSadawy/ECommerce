import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { TokenInterceptor} from './app/services/token.interceptor';
import { AuthService } from './app/services/auth.service';

import { provideHttpClient, withInterceptorsFromDi  } from '@angular/common/http';

const updatedAppConfig = {
  providers: [
    ...(appConfig.providers || []),
      AuthService,
      TokenInterceptor,
      provideHttpClient(withInterceptorsFromDi())
    ]
};

// bootstrapApplication(AppComponent, {
//   providers: [
//     AuthService,
//     TokenInterceptor,
//     provideHttpClient(
//       withInterceptorsFromDi() // Must use this to enable DI
//     )
//   ]
// }).catch((err) => console.error(err));

bootstrapApplication(AppComponent, updatedAppConfig)
  .catch((err) => console.error(err));
