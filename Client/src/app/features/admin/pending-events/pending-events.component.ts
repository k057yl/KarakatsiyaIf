import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../../core/services/admin.service';

@Component({
  selector: 'app-pending-events',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pending-events.component.html',
  styleUrls: ['./pending-events.component.scss']
})
export class PendingEventsComponent implements OnInit {
  private adminService = inject(AdminService);
  
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
        this.message.set('Ошибка загрузки ивентов');
        this.isLoading.set(false);
      }
    });
  }

  approve(eventId: string, isVip: boolean) {
    this.adminService.approveEvent(eventId, isVip).subscribe({
      next: () => {
        this.events.update(list => list.filter(e => e.id !== eventId));
        this.message.set(isVip ? 'Ивент одобрен как VIP 💎' : 'Ивент одобрен, пускаем в люди!');
        setTimeout(() => this.message.set(''), 3000);
      },
      error: (err) => {
        this.message.set(err.error?.message || 'Ошибка одобрения');
      }
    });
  }

  reject(eventId: string) {
    const reason = window.prompt('За что сносим ивент?');
    if (!reason) return;

    this.adminService.rejectEvent(eventId, reason).subscribe({
      next: () => {
        this.events.update(list => list.filter(e => e.id !== eventId));
        this.message.set('Ивент отклонён.');
        setTimeout(() => this.message.set(''), 3000);
      },
      error: (err) => {
        this.message.set(err.error?.message || 'Ошибка отклонения');
      }
    });
  }

  sendToFix(eventId: string) {
    const reason = window.prompt('Что оргу нужно исправить в ивенте?');
    if (!reason) return;

    this.adminService.sendToFix(eventId, reason).subscribe({
      next: () => {
        this.events.update(list => list.filter(e => e.id !== eventId));
        this.message.set('Ивент отправлен на доработку.');
        setTimeout(() => this.message.set(''), 3000);
      },
      error: (err) => this.message.set('Ошибка операции')
    });
  }

  deleteEvent(eventId: string) {
    if (!window.confirm('🚨 ВНИМАНИЕ: Это полностью удалит ивент из базы данных! Стираем?')) return;

    this.adminService.deleteEvent(eventId).subscribe({
      next: () => {
        this.events.update(list => list.filter(e => e.id !== eventId));
        this.message.set('Ивент полностью уничтожен.');
        setTimeout(() => this.message.set(''), 3000);
      },
      error: (err) => this.message.set('Ошибка удаления')
    });
  }
}