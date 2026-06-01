import { Component, inject, OnInit, signal, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { CreateEventModalComponent } from '../../events/create-event-modal/create-event-modal.component';
import { EditEventModalComponent } from '../../events/edit-event-modal/edit-event-modal.component';
import { EventService } from '../../events/services/event.service';

@Component({
  selector: 'app-organizer-dashboard',
  standalone: true,
  imports: [CommonModule, TranslateModule, CreateEventModalComponent, EditEventModalComponent],
  templateUrl: './organizer-dashboard.component.html',
  styleUrls: ['./organizer-dashboard.component.scss']
})
export class OrganizerDashboardComponent implements OnInit {
  private eventService = inject(EventService);
  private platformId = inject(PLATFORM_ID);

  public isModalOpen = signal(false);
  public organizerEvents = signal<any[]>([]);
  public isLoading = signal<boolean>(true);
  public selectedEventForEdit = signal<any | null>(null);

  ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {
      this.loadOrganizerEvents();
    }
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
        console.error('Failed to load organizer events:', err);
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
    console.log('Event processing completed successfully. ID:', eventId);
    this.isModalOpen.set(false);
    this.selectedEventForEdit.set(null);
    
    if (isPlatformBrowser(this.platformId)) {
      this.loadOrganizerEvents();
    }
  }
}