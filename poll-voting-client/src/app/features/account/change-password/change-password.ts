import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Auth } from '../../../core/services/auth';
import { CurrentUser } from '../../../core/services/current-user';

@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-change-password',
  templateUrl: './change-password.html',
  styleUrl: './change-password.scss'
})
export class ChangePassword {
  private fb = inject(FormBuilder);
  private authService = inject(Auth);
  private currentUser = inject(CurrentUser);
  private router = inject(Router);

  errorMessage = signal('');
  successMessage = signal('');

  form = this.fb.group({
    currentPassword: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(6)]]
  });

  async onSubmit(): Promise<void> {
    if (this.form.invalid) return;
    this.errorMessage.set('');
    try {
      const { currentPassword, newPassword } = this.form.getRawValue();
      await this.authService.changePassword(currentPassword!, newPassword!);
      this.successMessage.set('Password changed. Please log in again.');
      this.currentUser.clearAuth();
      setTimeout(() => this.router.navigate(['/login']), 1500);
    } catch (err: any) {
      this.errorMessage.set(err.error?.message ?? 'Failed to change password.');
    }
  }
}