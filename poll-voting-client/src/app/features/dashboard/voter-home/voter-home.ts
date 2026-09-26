import { Component, inject } from '@angular/core';
import { Auth } from '../../../core/services/auth';
import { CurrentUser } from '../../../core/services/current-user';
import { Router } from '@angular/router';

@Component({
  imports: [],
  selector: 'app-voter-home',
  styleUrl: './voter-home.scss',
  templateUrl: './voter-home.html',
})
export class VoterHome {
   currentUser = inject(CurrentUser);
  private authService = inject(Auth);
  private router = inject(Router);

  auth = this.currentUser.auth;

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
