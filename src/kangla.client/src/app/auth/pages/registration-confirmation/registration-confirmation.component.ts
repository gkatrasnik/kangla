import { Component } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotificationService } from '../../../core/notifications/notification.service';
import { ErrorService } from '../../../core/errors/error.service';
import { AuthService } from '../../../core/auth/auth.service';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-registration-confirmation',
  templateUrl: './registration-confirmation.component.html',
  styleUrls: ['./registration-confirmation.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule, 
    MatCardModule, 
    MatFormFieldModule, 
    MatInputModule, 
    MatButtonModule,
    RouterLink
  ],
})
export class RegistrationConfirmationComponent {
  resendConfirmationForm: FormGroup;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private notificationService: NotificationService,
    private authService: AuthService,
    private errorService: ErrorService
  ) {
    this.resendConfirmationForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  resendConfirmationEmail(): void {
    if (this.resendConfirmationForm.valid && !this.isSubmitting) {
      const { email } = this.resendConfirmationForm.value;
      this.isSubmitting = true;
      this.authService.resendConfirmationEmail(email).pipe(
        finalize(() => this.isSubmitting = false)
      ).subscribe({
        next: () => {
          this.notificationService.showNonErrorSnackBar('Confirmation email resent.');
        },
        error: (error) => {
          const { title, errors } = this.errorService.parseErrorResponse(error);
          const errorMessage = `${title}: ${errors.join(', ')}`;
          this.notificationService.showClientError(errorMessage);
        }
      });
    }
  }
}
