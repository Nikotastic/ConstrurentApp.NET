import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '@application/services/auth.service';
import { ToastService } from '@application/services/toast.service';

/**
 * Login Component - Presentation Layer
 * Handles user authentication
 */
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly toastService = inject(ToastService);

  loginForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.markFormGroupTouched(this.loginForm);
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const { email, password } = this.loginForm.value;

    this.authService.login(email, password).subscribe({
      next: () => {
        this.isLoading = false;
        this.toastService.success('Welcome back 👋');
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        this.isLoading = false;

        // Handle specific error cases
        let msg = 'Invalid credentials. Please try again.';

        if (error.status === 401) {
          // Unauthorized - Invalid credentials or inactive account
          const serverMsg = error.error?.message || '';
          if (serverMsg.toLowerCase().includes('inactive')) {
            msg = '⚠️ Your account is inactive. Please contact support.';
          } else if (serverMsg.toLowerCase().includes('credentials')) {
            msg =
              '⚠️ Invalid email or password. Please check your credentials and try again.';
          } else {
            msg = '⚠️ ' + serverMsg;
          }
        } else if (error.error?.message) {
          msg = error.error.message;
        }

        this.errorMessage = msg;
        this.toastService.error(msg);
      },
    });
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach((key) => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }
}
