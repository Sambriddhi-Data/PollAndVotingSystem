import { Component, inject, signal } from '@angular/core';
import { CurrentUser } from '../../core/services/current-user';
import { Auth } from '../../core/services/auth';
import { Router, RouterLink } from '@angular/router';

@Component({
  imports: [RouterLink],
  selector: 'app-navbar',
  styleUrl: './navbar.scss',
  templateUrl: './navbar.html',
})
export class Navbar {
  currentUser = inject(CurrentUser);
  private authService = inject(Auth);
  private router = inject(Router);

  auth = this.currentUser.auth;
  showLogoutConfirm = signal(false);

  homeLink():string{
    if (!this.currentUser.isLoggedIn()) return '/login';
    return this.currentUser.isAdmin() ? '/dashboard/admin' : '/dashboard/voter';

  }
    confirmLogout(): void {
    this.showLogoutConfirm.set(true);
  }

  cancelLogout(): void {
    this.showLogoutConfirm.set(false);
  }

  doLogout(): void {
    this.authService.logout();
    this.showLogoutConfirm.set(false);
    this.router.navigate(['/login']);
  }
}
