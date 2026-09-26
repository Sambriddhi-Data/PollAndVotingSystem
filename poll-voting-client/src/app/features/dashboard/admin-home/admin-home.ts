import { Component, inject } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { CurrentUser } from '../../../core/services/current-user';
import { Auth } from '../../../core/services/auth';

@Component({
  selector: 'app-admin-home',
  imports: [RouterLink],
  templateUrl: './admin-home.html',
  styleUrl: './admin-home.scss'
})
export class AdminHome {
  currentUser = inject(CurrentUser);
  private authService = inject(Auth);
  private router = inject(Router);

  auth = this.currentUser.auth;

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}