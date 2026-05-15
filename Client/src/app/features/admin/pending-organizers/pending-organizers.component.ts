import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService} from '../../../core/services/admin.service';
import { PendingOrganizer } from '../../../core/models/dtos/admin.dto';

@Component({
  selector: 'app-pending-organizers',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pending-organizers.component.html',
  styleUrls: ['./pending-organizers.component.scss']
})
export class PendingOrganizersComponent implements OnInit {
  private adminService = inject(AdminService);
  
  organizers = signal<PendingOrganizer[]>([]);
  isLoading = signal<boolean>(true);
  message = signal<string>('');

  ngOnInit() {
    this.loadPendingOrganizers();
  }

  loadPendingOrganizers() {
    this.isLoading.set(true);
    this.adminService.getPendingOrganizers().subscribe({
      next: (data) => {
        this.organizers.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.message.set('Ошибка загрузки данных');
        this.isLoading.set(false);
      }
    });
  }

  approve(organizerId: string) {
    this.adminService.approveOrganizer(organizerId).subscribe({
      next: (res) => {
        this.organizers.update(list => list.filter(o => o.organizerId !== organizerId));
        this.message.set('Красава, орг одобрен!');
        setTimeout(() => this.message.set(''), 3000);
      },
      error: (err) => {
        this.message.set(err.error?.message || 'Не удалось одобрить. Бэк капризничает.');
      }
    });
  }

  reject(organizerId: string) {
    const reason = window.prompt('За что посылаем нахуй? (Причина отказа)');

    if (!reason) {
      console.log('Отмена реджекта, админ передумал или не ввел причину.');
      return; 
    }

    this.adminService.rejectOrganizer(organizerId, reason).subscribe({
      next: (res) => {
        this.organizers.update(list => list.filter(o => o.organizerId !== organizerId));
        this.message.set('Заявка успешно послана нахуй (отклонена)!');
        setTimeout(() => this.message.set(''), 3000);
      },
      error: (err) => {
        this.message.set(err.error?.message || 'Не удалось отклонить. Бэк капризничает.');
      }
    });
  }
}