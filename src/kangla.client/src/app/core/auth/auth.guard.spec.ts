import { Router, UrlTree } from '@angular/router';
import { of } from 'rxjs';

import { AuthService } from './auth.service';
import { AuthGuard } from './auth.guard';

describe('AuthGuard', () => {
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;
  let guard: AuthGuard;

  beforeEach(() => {
    authService = jasmine.createSpyObj<AuthService>('AuthService', ['ensureAuthenticated']);
    router = jasmine.createSpyObj<Router>('Router', ['createUrlTree']);
    guard = new AuthGuard(authService, router);
  });

  it('allows navigation when the cached session is authenticated', () => {
    authService.ensureAuthenticated.and.returnValue(of(true));

    guard.canActivate().subscribe((result) => {
      expect(result).toBeTrue();
    });

    expect(router.createUrlTree).not.toHaveBeenCalled();
  });

  it('returns a login UrlTree when the session is anonymous', () => {
    const loginTree = {} as UrlTree;
    authService.ensureAuthenticated.and.returnValue(of(false));
    router.createUrlTree.and.returnValue(loginTree);

    guard.canActivate().subscribe((result) => {
      expect(result).toBe(loginTree);
    });

    expect(router.createUrlTree).toHaveBeenCalledOnceWith(['/login']);
  });
});
