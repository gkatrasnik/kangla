import { Component, ViewChild, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { HomeComponent } from './plants/pages/home/home.component';
import { DetailsComponent } from './plants/pages/details/details.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav'; 
import { MatNavList } from '@angular/material/list';
import { MatListItem } from '@angular/material/list';
import { AuthService } from './core/auth/auth.service';
import { Router, NavigationEnd } from '@angular/router';
import { MatSidenav } from '@angular/material/sidenav';
import { filter } from 'rxjs/operators';
import { LoadingIndicatorComponent } from './shared/components/loading-indicator/loading-indicator.component';
import { UserInfoDto } from './auth/user-info-dto';
import { version } from '../../package.json';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    HomeComponent,
    MatButtonModule,
    DetailsComponent,
    MatToolbarModule,
    MatIconModule,
    MatSidenavModule,
    MatNavList,
    MatListItem,
    MatSidenav,
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
  public version: string = version;

  @ViewChild('sidenav') sidenav!: MatSidenav;

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
    const currentUrl = this.router.url;
    return this.hiddenToolbarRoutes.includes(currentUrl);
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
    this.sidenav.close()
  }
}
