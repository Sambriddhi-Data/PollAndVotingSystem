import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { adminGuard } from './core/guards/admin-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then(m => m.Login)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register').then(m => m.Register)
  },
  {
    path: 'dashboard/admin',
    loadComponent: () => import('./features/dashboard/admin-home/admin-home').then(m => m.AdminHome),
    canActivate: [authGuard, adminGuard]
  },
  {
    path: 'dashboard/voter',
    loadComponent: () => import('./features/dashboard/voter-home/voter-home').then(m => m.VoterHome),
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: 'login' }
];