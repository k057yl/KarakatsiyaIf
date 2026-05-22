import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../../core/services/admin.service';

@Component({
  selector: 'app-active-events',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './active-events.component.html',
  styleUrls: ['./active-events.component.scss']
})
export class ActiveEventsComponent implements OnInit {
  private adminService = inject(AdminService);

  public events = signal<any[]>([]);
  public filteredEvents = signal<any[]>([]);
  public isLoading = signal<boolean>(true);
  public message = signal<string>('');
  
  public searchQuery = signal<string>('');
  public vipFilter = signal<string>('all');

  ngOnInit() {
    this.loadActiveEvents();
  }

  loadActiveEvents() {
    this.isLoading.set(true);
    this.adminService.getActiveEvents().subscribe({
      next: (data) => {
        this.events.set(data);
        this.applyFilters();
        this.isLoading.set(false);
      },
      error: () => {
        this.message.set('Не удалось поднять список активного движа');
        this.isLoading.set(false);
      }
    });
  }

  applyFilters() {
    let list = this.events();

    if (this.searchQuery()) {
      const q = this.searchQuery().toLowerCase();
      list = list.filter(e => e.title.toLowerCase().includes(q) || e.city.toLowerCase().includes(q));
    }

    if (this.vipFilter() === 'vip') {
      list = list.filter(e => e.isVip);
    } else if (this.vipFilter() === 'regular') {
      list = list.filter(e => !e.isVip);
    }

    this.filteredEvents.set(list);
  }

  onSearch(event: Event) {
    const target = event.target as HTMLInputElement;
    this.searchQuery.set(target.value);
    this.applyFilters();
  }

  onVipFilterChange(event: Event) {
    const target = event.target as HTMLSelectElement;
    this.vipFilter.set(target.value);
    this.applyFilters();
  }

  toggleVip(eventId: string) {
    this.adminService.toggleVip(eventId).subscribe({
      next: (res) => {
        this.events.update(list => list.map(e => e.id === eventId ? { ...e, isVip: res.isVip } : e));
        this.applyFilters();
        this.showToast('Статус тарифа изменен');
      }
    });
  }

  sendToFix(eventId: string) {
    const reason = window.prompt('Какую залупу оргу нужно исправить? Укажи причину:');
    if (!reason) return;

    this.adminService.sendToFix(eventId, reason).subscribe({
      next: () => {
        this.events.update(list => list.filter(e => e.id !== eventId));
        this.applyFilters();
        this.showToast('Ивент сбит на взлете и отправлен на ремонт');
      }
    });
  }

  deleteEvent(eventId: string) {
    if (!window.confirm('🚨 Сносим под ноль? Событие сотрется из базы навсегда!')) return;

    this.adminService.deleteEvent(eventId).subscribe({
      next: () => {
        this.events.update(list => list.filter(e => e.id !== eventId));
        this.applyFilters();
        this.showToast('Ивент успешно ликвидирован');
      }
    });
  }

  private showToast(msg: string) {
    this.message.set(msg);
    setTimeout(() => this.message.set(''), 3000);
  }
}