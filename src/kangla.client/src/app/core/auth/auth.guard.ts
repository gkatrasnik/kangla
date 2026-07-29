import { Injectable } from "@angular/core";
import { Router, UrlTree } from "@angular/router";
import { AuthService } from './auth.service'
import { Observable, map } from "rxjs";

@Injectable({ providedIn: 'root' })
export class AuthGuard {
  constructor(private authService: AuthService, private router: Router) { }

  canActivate(): Observable<boolean | UrlTree> {
    return this.authService.ensureAuthenticated().pipe(
      map((isSignedIn) => {
        return isSignedIn ? true : this.router.createUrlTree(['/login']);
      })
    );
  }
}
