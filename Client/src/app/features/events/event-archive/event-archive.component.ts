import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EventService } from '../../../core/services/event.service';
import { AdminService } from '../../../core/services/admin.service';
// Предполагаем, что у тебя есть AuthService для проверки роли
import { AuthService } from '../../../core/services/auth.service'; 

@Component({
  selector: 'app-event-archive',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './event-archive.component.html',
  styleUrls: ['./event-archive.component.scss']
})
export class EventArchiveComponent implements OnInit {
  private readonly eventService = inject(EventService);
  private readonly adminService = inject(AdminService);
  private readonly authService = inject(AuthService);

  public archivedEvents = signal<any[]>([]);
  public isLoading = signal<boolean>(true);
  public isAdmin = signal<boolean>(false);

  public ngOnInit(): void {
    this.isAdmin.set(this.authService.isSuperAdmin()); 
    this.loadArchivedEvents();
  }

  private loadArchivedEvents(): void {
    this.eventService.getArchivedEvents().subscribe({
      next: (data) => {
        this.archivedEvents.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  public deleteEvent(eventId: string): void {
    if (!window.confirm('Уничтожить этот хлам из базы навсегда?')) return;

    this.adminService.deleteEvent(eventId).subscribe({
      next: () => {
        this.archivedEvents.update(list => list.filter(e => e.id !== eventId));
      },
      error: (err) => {
        console.error(err);
      }
    });
  }
}