import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { CurrentUser } from '../services/current-user';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const currentUser = inject(CurrentUser);
  const router = inject(Router);
  const token = currentUser.getToken();

  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError(error => {
      if (error.status === 401) {
        // Covers: expired token, OR token invalidated by a password change (TokenVersion mismatch)
        currentUser.clearAuth();
        router.navigate(['/login']);
      }
      return throwError(() => error);
    })
  );
};