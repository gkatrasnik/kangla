import { Injectable } from '@angular/core';
import { HttpErrorResponse, HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { AuthService } from './auth.service'
import { Router } from '@angular/router';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  private readonly publicIdentityPaths = [
    '/login',
    '/register',
    '/refresh',
    '/forgotPassword',
    '/resetPassword',
    '/confirmEmail',
    '/resendConfirmationEmail',
    '/logout'
  ];

  constructor(private authService: AuthService, private router: Router) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const accessToken = localStorage.getItem('accessToken');
    const isPublicIdentityRequest = this.isPublicIdentityRequest(request);

    if (accessToken && !isPublicIdentityRequest) {
      request = this.addToken(request, accessToken);
    }

    return next.handle(request).pipe(
      catchError((error: unknown) => {
        if (error instanceof HttpErrorResponse
          && error.status === 401
          && accessToken
          && !isPublicIdentityRequest) {
          return this.handleTokenExpired(request, next);
        }

        return throwError(() => error);
      })
    );
  }

  private addToken(request: HttpRequest<any>, token: string): HttpRequest<any> {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  private handleTokenExpired(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return this.authService.refreshAccessToken().pipe(
      switchMap(() => {
        const newAccessToken = localStorage.getItem('accessToken');
        if (!newAccessToken) {
          this.authService.clearAuthentication();
          return throwError(() => new Error('Token refresh did not return an access token'));
        }

        return next.handle(this.addToken(request, newAccessToken)).pipe(
          catchError((error: unknown) => {
            if (error instanceof HttpErrorResponse && error.status === 401) {
              this.authService.clearAuthentication();
            }

            return throwError(() => error);
          })
        );
      }),
      catchError((error: unknown) => {
        if (!this.authService.hasStoredCredentials()) {
          void this.router.navigateByUrl('/login');
        }

        return throwError(() => error);
      })
    );
  }

  private isPublicIdentityRequest(request: HttpRequest<any>): boolean {
    const requestUrl = request.url.split('?')[0];
    return this.publicIdentityPaths.some((path) => requestUrl.endsWith(path));
  }
}
