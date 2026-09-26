import { Service, signal } from '@angular/core';
import { AuthResponse } from '../models/user.model';

const STORAGE_KEY = 'auth';

@Service()
export class CurrentUser {
  private authSignal = signal<AuthResponse | null>(this.loadFromStorage());

  auth = this.authSignal.asReadonly();

  private loadFromStorage(): AuthResponse | null {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? JSON.parse(raw) : null;
  }

  setAuth(auth: AuthResponse): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(auth));
    this.authSignal.set(auth);
  }

  clearAuth(): void {
    localStorage.removeItem(STORAGE_KEY);
    this.authSignal.set(null);
  }

  getToken(): string | null {
    return this.authSignal()?.token ?? null;
  }

  isLoggedIn(): boolean {
    return !!this.authSignal();
  }

  isAdmin(): boolean {
    return this.authSignal()?.role === 'Admin';
  }
}