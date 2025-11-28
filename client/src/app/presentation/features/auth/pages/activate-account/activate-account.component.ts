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
  selector: 'app-activate-account',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './activate-account.component.html',
  styleUrls: ['./activate-account.component.scss'],
})
export class ActivateAccountComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private authService = inject(AuthService);

  activateForm: FormGroup;
  userId: string | null = null;
  code: string | null = null;
  isLoading = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  constructor() {
    this.activateForm = this.fb.group(
      {
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', [Validators.required]],
      },
      { validators: this.passwordMatchValidator }
    );
  }

  ngOnInit(): void {
    this.userId = this.route.snapshot.queryParamMap.get('userId');
    this.code = this.route.snapshot.queryParamMap.get('code');

    if (!this.userId || !this.code) {
      this.errorMessage = 'Invalid activation link. Missing parameters.';
    }
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password')?.value === g.get('confirmPassword')?.value
      ? null
      : { mismatch: true };
  }

  onSubmit(): void {
    if (this.activateForm.invalid || !this.userId || !this.code) return;

    this.isLoading = true;
    this.errorMessage = null;

    const password = this.activateForm.get('password')?.value;

    this.authService
      .activateAccount(this.userId, this.code, password)
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.successMessage =
            'Account activated successfully! Redirecting to login...';
          setTimeout(() => {
            this.router.navigate(['/auth/login']);
          }, 3000);
        },
        error: (err) => {
          this.isLoading = false;
          this.errorMessage =
            err.error?.message ||
            'Failed to activate account. Please try again.';
        },
      });
  }
}
