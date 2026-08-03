import { Routes, mapToCanActivate } from '@angular/router';
import { DetailsComponent } from './plants/pages/details/details.component';
import { HomeComponent } from './plants/pages/home/home.component';
import { LoginComponent } from './auth/pages/login/login.component';
import { RegisterComponent } from './auth/pages/register/register.component';
import { AuthGuard } from './core/auth/auth.guard';
import { RegistrationConfirmationComponent } from './auth/pages/registration-confirmation/registration-confirmation.component';
import { PasswordResetComponent } from './auth/pages/password-reset/password-reset.component';
import { ForgotPasswordComponent } from './auth/pages/forgot-password/forgot-password.component';
import { WateringDevicesPageComponent } from './watering-devices/pages/watering-devices-page/watering-devices-page.component';
import { OverviewComponent } from './plants/pages/overview/overview.component';

export const routes: Routes = [
    { path: '', redirectTo: '/home', pathMatch: 'full'},
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'registration-confirmation', component: RegistrationConfirmationComponent },
    { path: 'forgot-password', component: ForgotPasswordComponent },
    { path: 'password-reset', component: PasswordResetComponent },
    { path: 'home', component: OverviewComponent, canActivate: mapToCanActivate([AuthGuard])  },
    { path: 'plants', component: HomeComponent, canActivate: mapToCanActivate([AuthGuard])  },
    { path: 'details/:id', component: DetailsComponent, canActivate: mapToCanActivate([AuthGuard])  },
    { path: 'watering-devices', component: WateringDevicesPageComponent, canActivate: mapToCanActivate([AuthGuard]) },
    { path: '**', redirectTo: '/home' },
];
