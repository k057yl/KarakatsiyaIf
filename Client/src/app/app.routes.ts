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

  {
    path: 'become-organizer',
    loadComponent: () => import('./features/organizer/become-organizer/become-organizer.component').then(m => m.BecomeOrganizerComponent)
  },
  {
    path: 'organizer/dashboard',
    loadComponent: () => import('./features/organizer/dashboard/organizer-dashboard.component').then(m => m.OrganizerDashboardComponent)
  },

  { 
    path: 'events', 
    loadComponent: () => import('./features/events/event-list/event-list.component').then(m => m.EventListComponent) 
  },
  { path: '', redirectTo: 'events', pathMatch: 'full' },
  { path: '**', redirectTo: 'events' }
];