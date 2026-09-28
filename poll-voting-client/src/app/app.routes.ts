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
  {
    path: 'elections',
    loadComponent: () => import('./features/elections/election-list/election-list').then(m => m.ElectionList),
    canActivate: [authGuard]
  },
  {
    path: 'elections/new',
    loadComponent: () => import('./features/elections/election-form/election-form').then(m => m.ElectionForm),
    canActivate: [authGuard, adminGuard]
  },
  {
    path: 'elections/:id/edit',
    loadComponent: () => import('./features/elections/election-form/election-form').then(m => m.ElectionForm),
    canActivate: [authGuard, adminGuard]
  },
  {
    path: 'elections/:id',
    loadComponent: () => import('./features/elections/election-detail/election-detail').then(m => m.ElectionDetail),
    canActivate: [authGuard]
  },
    {
    path: 'forgot-password',
    loadComponent: () => import('./features/auth/forgot-password/forgot-password').then(m => m.ForgotPassword)
  },
  {
    path: 'reset-password',
    loadComponent: () => import('./features/auth/reset-password/reset-password').then(m => m.ResetPassword)
  },
  {
    path: 'account/change-password',
    loadComponent: () => import('./features/account/change-password/change-password').then(m => m.ChangePassword),
    canActivate: [authGuard]
  },
  {
    path: 'admin/audit-logs',
    loadComponent: () => import('./features/admin/audit-log-page/audit-log-page').then(m => m.AuditLogPage),
    canActivate: [authGuard, adminGuard]
  },
  {
    path: 'notifications',
    loadComponent: () => import('./features/notifications/notifications-page/notifications-page').then(m => m.NotificationsPage),
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: 'login' },

];