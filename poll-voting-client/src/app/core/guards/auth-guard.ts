import { CanActivateFn, Router } from '@angular/router';
import { CurrentUser } from '../services/current-user';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
 const currentUser = inject(CurrentUser);
 const router = inject(Router);
 
  if (currentUser.isLoggedIn()) {
    return true;
  } else {
    router.navigate(['/login']);
    return false;
  }

};
