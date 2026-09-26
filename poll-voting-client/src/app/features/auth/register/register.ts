import { Component, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Auth } from '../../../core/services/auth';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class Register implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(Auth);
  private http = inject(HttpClient);
  private router = inject(Router);

  errorMessage = signal('');
  isSubmitting = signal(false);
  departments = signal<{ id: number; name: string }[]>([]);

  form = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    departmentId: [null as number | null, Validators.required],
    dateJoined: ['', Validators.required]
  });

  async ngOnInit(): Promise<void> {
    try {
      const data = await firstValueFrom(
        this.http.get<{ id: number; name: string }[]>(`${environment.apiUrl}/departments`)
      );
      this.departments.set(data);
    } catch {
      this.departments.set([]);
    }
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) return;

    this.errorMessage.set('');
    this.isSubmitting.set(true);

    try {
      await this.authService.register(this.form.getRawValue() as any);
      this.router.navigate(['/dashboard/voter']);
    } catch (err: any) {
      this.errorMessage.set(err.error?.message ?? 'Registration failed.');
    } finally {
      this.isSubmitting.set(false);
    }
  }
}