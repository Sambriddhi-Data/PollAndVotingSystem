import { CanActivateFn, Router } from '@angular/router';
import { CurrentUser } from '../services/current-user';
import { inject } from '@angular/core';

export const adminGuard: CanActivateFn = (route, state) => {
  
  const currentUser = inject(CurrentUser);
  const router = inject(Router);

  if (currentUser.isAdmin()){
    return true;
  } else {
    router.navigate(['/dashboard']);
    return false;
  }
};
