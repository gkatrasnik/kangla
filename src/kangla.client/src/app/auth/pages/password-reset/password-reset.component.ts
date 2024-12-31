import { Component } from '@angular/core';
import { ReactiveFormsModule,FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { NotificationService } from '../../../core/notifications/notification.service';
import { AuthService } from '../../../core/auth/auth.service';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { ErrorService } from '../../../core/errors/error.service';

@Component({
  selector: 'app-password-reset',
  templateUrl: './password-reset.component.html',
  styleUrls: ['./password-reset.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule]
})
export class PasswordResetComponent{
  resetForm: FormGroup;
  resetCode: string | null = null; // Assume this will be set by some mechanism, e.g., query param

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router,
    private notificationService: NotificationService,
    private authService: AuthService,
    private errorService: ErrorService
  ) {
    this.resetForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      resetCode: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.resetForm.valid) {
      const { email, resetCode, newPassword } = this.resetForm.value;
      const trimmedResetCode = resetCode.trim();
      this.authService.resetPassword(email, trimmedResetCode, newPassword).subscribe({
        next: () => {
          this.notificationService.showNonErrorSnackBar('Password has been reset successfully.');
          this.router.navigate(['/login'], { replaceUrl: true }); // Redirect to login page after successful reset
        },
        error: (error) => {
          const { title, errors } = this.errorService.parseErrorResponse(error);
          const errorMessage = `${errors.join(', ')}`;
          this.notificationService.showServerError(title, errorMessage);
        }
      });
    }
  }
}
