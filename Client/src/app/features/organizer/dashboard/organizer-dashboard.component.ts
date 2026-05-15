import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CreateEventModalComponent } from '../create-event-modal/create-event-modal.component';

@Component({
  selector: 'app-organizer-dashboard',
  standalone: true,
  imports: [CommonModule, CreateEventModalComponent],
  templateUrl: './organizer-dashboard.component.html',
  styleUrls: ['./organizer-dashboard.component.scss']
})
export class OrganizerDashboardComponent {
  isModalOpen = signal(false);

  onEventCreated(eventId: string) {
    console.log('Заебись, ивент создан! ID:', eventId);
    this.isModalOpen.set(false);
    // TODO: обновить список ивентов
  }
}