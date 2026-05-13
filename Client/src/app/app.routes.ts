import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },
  {
    path: 'admin/pending',
    loadComponent: () => import('./features/admin/pending-organizers/pending-organizers.component').then(m => m.PendingOrganizersComponent)
  },
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' }
];