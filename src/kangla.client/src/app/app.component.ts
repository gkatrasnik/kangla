import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from './core/auth/auth.service';
import { Router, NavigationEnd } from '@angular/router';
import { catchError, filter } from 'rxjs/operators';
import { LoadingIndicatorComponent } from './shared/components/loading-indicator/loading-indicator.component';
import { UserInfoDto } from './auth/user-info-dto';
import packageInfo from '../../package.json';
import { of } from 'rxjs';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatButtonModule,
    MatToolbarModule,
    MatIconModule,
    MatMenuModule,
    MatDividerModule,
    LoadingIndicatorComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit{
  title = 'kangla';
  showToolbar: boolean = true;
  userInfo: UserInfoDto | null = null;
  private hiddenToolbarRoutes: string[] = ['/login', '/register', '/registration-confirmation', '/forgot-password', '/password-reset'];
  public version: string = packageInfo.version;

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.showToolbar = !this.isHiddenToolbarRoute();
    });
    
    this.authService.userInfo$.subscribe((info) => {
      this.userInfo = info;
    });
  }

  isHiddenToolbarRoute(): boolean {
    const currentUrl = this.router.url.split('?')[0].split('#')[0];
    return this.hiddenToolbarRoutes.includes(currentUrl);
  }

  logout() {
    this.authService.logout().pipe(
      catchError((error) => {
        console.error("Logout failed:", error);
        return of(true); // Ensure navigation happens even if logout fails
      })
    ).subscribe(() => {
      this.router.navigate(['/login']);
    });

  }
}
