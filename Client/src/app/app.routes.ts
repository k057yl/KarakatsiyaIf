import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },
  {
    path: 'admin',
    loadComponent: () => import('./features/admin/admin-layout/admin-layout.component').then(m => m.AdminLayoutComponent),
    children: [
      { path: '', redirectTo: 'organizers', pathMatch: 'full' },
      {
        path: 'organizers',
        loadComponent: () => import('./features/admin/pending-organizers/pending-organizers.component').then(m => m.PendingOrganizersComponent)
      },
      {
        path: 'events',
        loadComponent: () => import('./features/admin/pending-events/pending-events.component').then(m => m.PendingEventsComponent)
      }
    ]
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
  {
    path: 'events/:id',
    loadComponent: () => import('./features/events/event-details/event-details.component').then(m => m.EventDetailsComponent)
  },
  { path: '', redirectTo: 'events', pathMatch: 'full' },
  { path: '**', redirectTo: 'events' }
];