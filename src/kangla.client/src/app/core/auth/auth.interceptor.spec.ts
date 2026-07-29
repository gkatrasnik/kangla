import {
  HttpErrorResponse,
  HttpHandler,
  HttpRequest,
  HttpResponse
} from '@angular/common/http';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';

import { AuthService } from './auth.service';
import { TokenInterceptor } from './auth.interceptor';

describe('TokenInterceptor', () => {
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;
  let interceptor: TokenInterceptor;

  beforeEach(() => {
    localStorage.clear();
    authService = jasmine.createSpyObj<AuthService>(
      'AuthService',
      ['refreshAccessToken', 'clearAuthentication', 'hasStoredCredentials']
    );
    router = jasmine.createSpyObj<Router>('Router', ['navigateByUrl']);
    interceptor = new TokenInterceptor(authService, router);
  });

  afterEach(() => localStorage.clear());

  it('adds the bearer token to protected API requests', () => {
    localStorage.setItem('accessToken', 'access-token');
    const next = jasmine.createSpyObj<HttpHandler>('HttpHandler', ['handle']);
    next.handle.and.returnValue(of(new HttpResponse({ status: 200 })));

    interceptor.intercept(new HttpRequest('GET', '/api/plants'), next).subscribe();

    const forwardedRequest = next.handle.calls.mostRecent().args[0];
    expect(forwardedRequest.headers.get('Authorization')).toBe('Bearer access-token');
  });

  it('does not refresh or clear authentication for a 429 response', () => {
    localStorage.setItem('accessToken', 'access-token');
    const next = jasmine.createSpyObj<HttpHandler>('HttpHandler', ['handle']);
    next.handle.and.returnValue(throwError(() => new HttpErrorResponse({
      status: 429,
      statusText: 'Too Many Requests'
    })));

    interceptor.intercept(new HttpRequest('GET', '/api/plants'), next).subscribe({
      error: (error: HttpErrorResponse) => expect(error.status).toBe(429)
    });

    expect(authService.refreshAccessToken).not.toHaveBeenCalled();
    expect(authService.clearAuthentication).not.toHaveBeenCalled();
    expect(router.navigateByUrl).not.toHaveBeenCalled();
  });
});
