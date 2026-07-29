import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [
        AuthService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(AuthService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
    localStorage.clear();
  });

  it('checks the session only once and reuses the authenticated state', () => {
    localStorage.setItem('accessToken', 'access-token');
    localStorage.setItem('refreshToken', 'refresh-token');

    let firstResult = false;
    service.ensureAuthenticated().subscribe((result) => firstResult = result);

    const sessionRequest = httpTesting.expectOne('/api/manage/info');
    sessionRequest.flush({ email: 'user@example.com', isEmailConfirmed: true });

    expect(firstResult).toBeTrue();

    let secondResult = false;
    service.ensureAuthenticated().subscribe((result) => secondResult = result);

    expect(secondResult).toBeTrue();
    httpTesting.expectNone('/api/manage/info');
  });

  it('shares one refresh request between concurrent callers', () => {
    localStorage.setItem('refreshToken', 'old-refresh-token');

    let completedRequests = 0;
    service.refreshAccessToken().subscribe(() => completedRequests++);
    service.refreshAccessToken().subscribe(() => completedRequests++);

    const refreshRequest = httpTesting.expectOne('/api/refresh');
    refreshRequest.flush({
      accessToken: 'new-access-token',
      refreshToken: 'new-refresh-token'
    });

    expect(completedRequests).toBe(2);
    expect(localStorage.getItem('accessToken')).toBe('new-access-token');
    expect(localStorage.getItem('refreshToken')).toBe('new-refresh-token');
  });

  it('does not clear a local session after a transient profile error', () => {
    localStorage.setItem('accessToken', 'access-token');
    localStorage.setItem('refreshToken', 'refresh-token');

    let result = false;
    service.ensureAuthenticated().subscribe((authenticated) => result = authenticated);

    const sessionRequest = httpTesting.expectOne('/api/manage/info');
    sessionRequest.flush('Too Many Requests', {
      status: 429,
      statusText: 'Too Many Requests'
    });

    expect(result).toBeTrue();
    expect(localStorage.getItem('accessToken')).toBe('access-token');
    expect(localStorage.getItem('refreshToken')).toBe('refresh-token');

    service.ensureAuthenticated().subscribe((authenticated) => result = authenticated);
    expect(result).toBeTrue();
    httpTesting.expectNone('/api/manage/info');
  });
});
