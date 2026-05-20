import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EventService } from '../../../core/services/event.service';

@Component({
  selector: 'app-event-archive',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './event-archive.component.html',
  styleUrls: ['./event-archive.component.scss']
})
export class EventArchiveComponent implements OnInit {
  private readonly eventService = inject(EventService);

  public archivedEvents = signal<any[]>([]);
  public isLoading = signal<boolean>(true);

  public ngOnInit(): void {
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
}