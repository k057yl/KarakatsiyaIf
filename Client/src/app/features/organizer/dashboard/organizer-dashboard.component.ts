import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CreateEventModalComponent } from '../../events/create-event/create-event-modal.component';
import { EventService } from '../../../core/services/event.service';

@Component({
  selector: 'app-organizer-dashboard',
  standalone: true,
  imports: [CommonModule, CreateEventModalComponent],
  templateUrl: './organizer-dashboard.component.html',
  styleUrls: ['./organizer-dashboard.component.scss']
})
export class OrganizerDashboardComponent implements OnInit {
  private eventService = inject(EventService);

  public isModalOpen = signal(false);
  public organizerEvents = signal<any[]>([]);
  public isLoading = signal<boolean>(true);
  public selectedEventForEdit = signal<any | null>(null);

  ngOnInit() {
    this.loadOrganizerEvents();
  }

  public loadOrganizerEvents() {
    this.isLoading.set(true);
    this.eventService.getOrganizerEvents().subscribe({
      next: (data: any[]) => {
        const pendingEvents = data.filter(e => [0, 1, 3].includes(e.status));
        
        this.organizerEvents.set(pendingEvents);
        this.isLoading.set(false);
      },
      error: (err: unknown) => {
        console.error('Не удалось загрузить ваши события', err);
        this.isLoading.set(false);
      }
    });
  }

  public openCreateModal() {
    this.selectedEventForEdit.set(null);
    this.isModalOpen.set(true);
  }

  public openEditModal(eventItem: any) {
    this.selectedEventForEdit.set(eventItem);
    this.isModalOpen.set(true);
  }

  onEventCreated(eventId: string) {
    console.log('Заебись, ивент обработан! ID:', eventId);
    this.isModalOpen.set(false);
    this.loadOrganizerEvents();
  }
}