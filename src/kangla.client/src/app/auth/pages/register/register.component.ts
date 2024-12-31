import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../../core/auth/auth.service';
import { ErrorService } from '../../../core/errors/error.service';
import { NotificationService } from '../../../core/notifications/notification.service';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-register',
    imports: [
        CommonModule,
        MatCardModule,
        MatInputModule,
        MatButtonModule,
        MatFormFieldModule,
        ReactiveFormsModule,
        RouterLink
    ],
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss']
})

export class RegisterComponent implements OnInit {
  registerForm: FormGroup;

  constructor(
    private fb: FormBuilder, 
    private authService: AuthService, 
    private router: Router,
    private errorService: ErrorService,
    private notificationService: NotificationService
  ) {
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]], //match requirements with API
      confirmPassword: ['', [Validators.required]]
    }, { validator: this.mustMatch('password', 'confirmPassword') });
  }

  ngOnInit(): void { }

  onSubmit(): void {
    if (this.registerForm.valid) {
      const { email, password } = this.registerForm.value;
      this.authService.register(email, password).subscribe({
        next: (response) => {
          console.log('User registered successfully', response);
          this.notificationService.showNonErrorSnackBar('Registration successfull.');
          this.router.navigate(['/registration-confirmation'], { replaceUrl: true });
        },
        error: (error) => {
          const { title, errors, statusCode } = this.errorService.parseErrorResponse(error);
          const errorMessage = `${errors.join(', ')}`;
          this.notificationService.showServerError(title, errorMessage);

          //TODO if error was with sending mail, it must navigate to registration-confirmation, where user can resend confirmation email, else do nothing
          if (statusCode == 400){
            return;
          }           
          
          this.router.navigate(['/registration-confirmation'], { replaceUrl: true });
        }
      });
    }
  }

  mustMatch(controlName: string, matchingControlName: string) {
    return (formGroup: FormGroup) => {
      const control = formGroup.controls[controlName];
      const matchingControl = formGroup.controls[matchingControlName];

      if (matchingControl.errors && !matchingControl.errors['mustMatch']) {
        return;
      }

      if (control.value !== matchingControl.value) {
        matchingControl.setErrors({ mustMatch: true });
      } else {
        matchingControl.setErrors(null);
      }
    };
  }
}
