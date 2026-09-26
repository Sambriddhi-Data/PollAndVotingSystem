import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../../core/services/auth';
import { CurrentUser } from '../../../core/services/current-user';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  private fb = inject(FormBuilder);
  private authService = inject(Auth);
  private currentUser = inject(CurrentUser);
  private router = inject(Router);

  errorMessage = signal('');
  isSubmitting = signal(false);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  async onSubmit(): Promise<void> {
    if (this.form.invalid) return;

    this.errorMessage.set('');
    this.isSubmitting.set(true);

    try {
      await this.authService.login(this.form.getRawValue() as any);
      const target = this.currentUser.isAdmin() ? '/dashboard/admin' : '/dashboard/voter';
      this.router.navigate([target]);
    } catch (err: any) {
      this.errorMessage.set(err.error?.message ?? 'Login failed. Please try again.');
    } finally {
      this.isSubmitting.set(false);
    }
  }
}