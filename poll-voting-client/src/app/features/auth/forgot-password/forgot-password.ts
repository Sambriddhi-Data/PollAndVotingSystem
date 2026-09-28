import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../../core/services/auth';

@Component({
  imports: [ReactiveFormsModule, RouterLink],
  selector: 'app-forgot-password',
  styleUrl: './forgot-password.scss',
  templateUrl: './forgot-password.html'
})
export class ForgotPassword {
  private fb = inject(FormBuilder);
  private authService = inject(Auth);
  private router = inject(Router);

  message = signal('');
  devOtp = signal('');
  errorMessage = signal('');

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  async onSubmit(): Promise<void> {
    if (this.form.invalid) return;
    this.errorMessage.set('');
    try {
      const result = await this.authService.forgotPassword(this.form.value.email!);
      this.message.set(result.message);
      if (result.devOtp) {
        this.devOtp.set(result.devOtp);
      }
    } catch (err: any) {
      this.errorMessage.set(err.error?.message ?? 'Something went wrong.');
    }
  }

  goToReset(): void {
    this.router.navigate(['/reset-password'], { queryParams: { email: this.form.value.email } });
  }
}