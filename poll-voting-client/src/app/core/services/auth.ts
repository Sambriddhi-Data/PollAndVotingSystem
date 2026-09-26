import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/user.model';
import { CurrentUser } from './current-user';

@Service()
export class Auth {
  private http = inject(HttpClient);
  private currentUser = inject(CurrentUser);
  private baseUrl = `${environment.apiUrl}/auth`;

  async register(request: RegisterRequest): Promise<AuthResponse> {
    const response = await firstValueFrom(
      this.http.post<AuthResponse>(`${this.baseUrl}/register`, request)
    );
    this.currentUser.setAuth(response);
    return response;
  }

  async login(request: LoginRequest): Promise<AuthResponse> {
    const response = await firstValueFrom(
      this.http.post<AuthResponse>(`${this.baseUrl}/login`, request)
    );
    this.currentUser.setAuth(response);
    return response;
  }

  logout(): void {
    this.currentUser.clearAuth();
  }
}