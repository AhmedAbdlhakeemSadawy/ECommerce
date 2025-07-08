import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { AuthService,RefreshTokenRequest } from './auth.service';
import { Router } from '@angular/router';


@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

  private excludedUrls = ['/Account/login'];
  constructor(private authService: AuthService ,private router: Router) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const isExcluded = this.excludedUrls.some(url => req.url.includes(url));
    if (isExcluded) {
      return next.handle(req);
    }
    let authReq = req;
    console.log('TokenInterceptor fired for:', req.url);
    const token = this.authService.getToken();
    if (token) {
      authReq = this.addTokenHeader(req, token);
    }
    return next.handle(authReq).pipe(
      catchError(error => {
        if (error instanceof HttpErrorResponse && error.status === 401) {
          return this.handle401Error(authReq, next);
        }
        return throwError(() => error);
      })
    );
  }

  private addTokenHeader(request: HttpRequest<any>, token: string) {
    console.log('addTokenHeader')
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }


  private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null); // Signal that no token is available yet
  
      const refreshToken = this.authService.getRefreshToken();
      const accessToken = this.authService.getToken();
  
      if (refreshToken && accessToken) {
        const refreshTokenRequest: RefreshTokenRequest = { accessToken, refreshToken };
  
        return this.authService.refreshToken(refreshTokenRequest).pipe(
          switchMap((response: { accessToken: string; refreshToken: string }) => {
            this.isRefreshing = false;
            this.authService.setToken(response.accessToken);
            this.authService.setRefreshToken(response.refreshToken);
            this.refreshTokenSubject.next(response.accessToken); // Broadcast new token to waiting requests
            console.log(response.accessToken)
            return next.handle(this.addTokenHeader(request, response.accessToken)).pipe(
              catchError((retryError) => {
                // Handle errors from the retried request (e.g., add order)
                // Don't logout here unless it's a 401
                console.log(retryError);
                if (retryError.status === 401) {
                  this.authService.logout();
                   this.router.navigate(['/login']);
                  return throwError(() => new Error('Session expired. Please log in again.'));
                }
                // For non-401 errors, pass the error through
                return throwError(() => retryError);
              })
            );
          })
        );
      } else {
        this.isRefreshing = false;
        this.refreshTokenSubject.next(null);
        this.authService.logout();
        this.router.navigate(['/login']);
        return throwError(() => new Error('No refresh token available.'));
      }
    } else {
      // Wait for the ongoing refresh to complete
      return this.refreshTokenSubject.pipe(
        filter((token) => token !== null), // Proceed only when a valid token is emitted
        take(1), // Complete after one valid token
        switchMap((token) => next.handle(this.addTokenHeader(request, token!))) // Retry with new token
      );
    }
  }
} 