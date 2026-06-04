import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { AuthService } from '../../auth/services/auth.service';
import { EventService } from '../services/event.service';

@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  templateUrl: './event-list.component.html',
  styleUrls: ['./event-list.component.scss']
})
export class EventListComponent implements OnInit {
  public authService = inject(AuthService);
  private eventService = inject(EventService);

  public events = signal<any[]>([]);
  public isLoading = signal<boolean>(true);
  
  public currentPage = signal<number>(1);
  public pageSize = signal<number>(6);

  private currentUserSignal = toSignal(this.authService.currentUser$);

  public isVisitor = computed(() => {
    const user = this.currentUserSignal();
    return !user || user.role === 'Visitor';
  });

  public pagedEvents = computed(() => {
    const startIndex = (this.currentPage() - 1) * this.pageSize();
    return this.events().slice(startIndex, startIndex + this.pageSize());
  });

  public totalEventsCount = computed(() => this.events().length);
  public totalPagesCount = computed(() => Math.ceil(this.totalEventsCount() / this.pageSize()) || 1);

  ngOnInit() {
    this.eventService.getApprovedEvents().subscribe({
      next: (data: any[]) => {
        this.events.set(data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  public changePage(page: number): void {
    if (page >= 1 && page <= this.totalPagesCount()) {
      this.currentPage.set(page);
    }
  }
}