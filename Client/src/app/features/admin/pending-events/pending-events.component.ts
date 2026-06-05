import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../services/admin.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-pending-events',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './pending-events.component.html',
  styleUrls: ['./pending-events.component.scss']
})
export class PendingEventsComponent implements OnInit {
  private adminService = inject(AdminService);
  private translate = inject(TranslateService);
  
  events = signal<any[]>([]);
  isLoading = signal<boolean>(true);
  message = signal<string>('');

  ngOnInit() {
    this.loadPendingEvents();
  }

  loadPendingEvents() {
    this.isLoading.set(true);
    this.adminService.getPendingEvents().subscribe({
      next: (data: any[]) => {
        this.events.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.message.set('ADMIN_PERFORMERS.LOADING');
        this.isLoading.set(false);
      }
    });
  }

  approve(eventId: string, isVip: boolean) {
    if (!eventId) {
      console.error('Ошибка: eventId равен undefined! Проверь свойства DTO бэкенда.');
      return;
    }

    this.adminService.approveEvent(eventId, isVip).subscribe({
      next: () => {
        this.events.update(list => list.filter(e => (e.id || e.eventId) !== eventId));
        this.message.set(isVip ? 'SUCCESS.EVENT_VIP_TOGGLED' : 'SUCCESS.EVENT_APPROVED');
        setTimeout(() => this.message.set(''), 3000);
      },
      error: (err) => {
        this.message.set(err.error?.message || 'ERRORS.VALIDATION_FAILED');
      }
    });
  }

  reject(eventId: string) {
    if (!eventId) return;

    const reason = window.prompt('За что сносим ивент?');
    if (!reason) return;

    this.adminService.rejectEvent(eventId, reason).subscribe({
      next: () => {
        this.events.update(list => list.filter(e => (e.id || e.eventId) !== eventId));
        this.message.set('SUCCESS.EVENT_REJECTED');
        setTimeout(() => this.message.set(''), 3000);
      },
      error: (err) => {
        this.message.set(err.error?.message || 'ERRORS.VALIDATION_FAILED');
      }
    });
  }

  sendToFix(eventId: string) {
    if (!eventId) return;

    const reason = window.prompt('Что оргу нужно исправить в ивенте?');
    if (!reason) return;

    this.adminService.sendToFix(eventId, reason).subscribe({
      next: () => {
        this.events.update(list => list.filter(e => (e.id || e.eventId) !== eventId));
        this.message.set('SUCCESS.EVENT_SENT_TO_FIX');
        setTimeout(() => this.message.set(''), 3000);
      },
      error: (err) => this.message.set('ERRORS.INTERNAL_SERVER_ERROR')
    });
  }

  deleteEvent(eventId: string) {
    if (!eventId) return;

    const confirmMsg = this.translate.instant('ADMIN_PERFORMERS.DELETE_CONFIRM');
    if (!window.confirm(confirmMsg)) return;

    this.adminService.deleteEvent(eventId).subscribe({
      next: () => {
        this.events.update(list => list.filter(e => (e.id || e.eventId) !== eventId));
        this.message.set('SUCCESS.EVENT_DELETED');
        setTimeout(() => this.message.set(''), 3000);
      },
      error: (err) => this.message.set('ERRORS.INTERNAL_SERVER_ERROR')
    });
  }
}