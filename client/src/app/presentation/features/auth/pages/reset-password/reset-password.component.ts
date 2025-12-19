import { Component, OnInit, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../../application/services/auth.service';
@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './reset-password.component.html',
})
// Reset password component
export class ResetPasswordComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private authService = inject(AuthService);
  resetPasswordForm: FormGroup;
  userId: string | null = null;
  code: string | null = null;
  isLoading = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  constructor() {
    // Initialize form
    this.resetPasswordForm = this.fb.group(
      {
        newPassword: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', [Validators.required]],
      },
      { validators: this.passwordMatchValidator }
    );
  }
  
  ngOnInit(): void {
    this.userId = this.route.snapshot.queryParamMap.get('userId');
    this.code = this.route.snapshot.queryParamMap.get('code');
    if (!this.userId || !this.code) {
      this.errorMessage = 'Invalid reset link. Missing parameters.';
    }
  }
  passwordMatchValidator(g: FormGroup) {
    return g.get('newPassword')?.value === g.get('confirmPassword')?.value
      ? null
      : { mismatch: true };
  }
  onSubmit(): void {
    if (this.resetPasswordForm.invalid || !this.userId || !this.code) return;
    this.isLoading = true;
    this.errorMessage = null;
    const newPassword = this.resetPasswordForm.get('newPassword')?.value;
    this.authService
      .resetPassword(this.userId, this.code, newPassword)
      .subscribe({
        next: (response) => {
          this.isLoading = false;
          this.successMessage =
            response.message ||
            'Password reset successfully! Redirecting to login...';
          setTimeout(() => {
            this.router.navigate(['/auth/login']);
          }, 3000);
        },
        error: (err) => {
          this.isLoading = false;
          this.errorMessage =
            err.error?.message ||
            err.error?.errors?.join(', ') ||
            'Failed to reset password. Please try again or request a new reset link.';
        },
      });
  }
  // Mark form group as touched
  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach((key) => {
      const control = formGroup.get(key);
      control?.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }
}
