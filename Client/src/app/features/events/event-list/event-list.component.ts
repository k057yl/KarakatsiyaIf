import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { EventService } from '../services/event.service';

@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './event-list.component.html',
  styleUrls: ['./event-list.component.scss']
})
export class EventListComponent implements OnInit {
  authService = inject(AuthService);
  eventService = inject(EventService);

  events = signal<any[]>([]);
  isLoading = signal(true);

  ngOnInit() {
    this.eventService.getApprovedEvents().subscribe({
      next: (data: any[]) => {
        this.events.set(data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  isVisitor() {
    let role: string | undefined;
    this.authService.currentUser$.subscribe(u => role = u?.role);
    return role === 'Visitor' || !role;
  }
}