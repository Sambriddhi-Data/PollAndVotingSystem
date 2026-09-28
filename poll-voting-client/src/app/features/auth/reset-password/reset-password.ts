import { Component, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Auth } from '../../../core/services/auth';

@Component({
  imports: [ReactiveFormsModule, RouterLink],
  selector: 'app-reset-password',
  styleUrl: './reset-password.scss',
  templateUrl: './reset-password.html'
})
export class ResetPassword implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(Auth);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  errorMessage = signal('');
  successMessage = signal('');

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    otp: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(6)]]
  });

  ngOnInit(): void {
    const email = this.route.snapshot.queryParamMap.get('email');
    if (email) this.form.patchValue({ email });
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) return;
    this.errorMessage.set('');
    try {
      const { email, otp, newPassword } = this.form.getRawValue();
      await this.authService.resetPassword(email!, otp!, newPassword!);
      this.successMessage.set('Password reset. Redirecting to login...');
      setTimeout(() => this.router.navigate(['/login']), 1500);
    } catch (err: any) {
      this.errorMessage.set(err.error?.message ?? 'Reset failed.');
    }
  }
}