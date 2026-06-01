import { Component, inject, OnInit, signal, computed, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EventService } from '../services/event.service';

export interface CalendarDay {
  date: number;
  fullDateString: string;
  isOccupied: boolean;
  isToday: boolean;
  isPast: boolean;
}

@Component({
  selector: 'app-event-calendar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './event-calendar.component.html',
  styleUrls: ['./event-calendar.component.scss']
})
export class EventCalendarComponent implements OnInit {
  private eventService = inject(EventService);

  public currentDate = signal(new Date());
  public occupiedDates = signal<string[]>([]);

  public dateSelected = output<string>();

  public currentMonthName = computed(() => {
    return this.currentDate().toLocaleString('ru-RU', { month: 'long' });
  });
  public currentYear = computed(() => this.currentDate().getFullYear());
  public calendarGrid = computed(() => {
    const date = this.currentDate();
    const year = date.getFullYear();
    const month = date.getMonth();

    const firstDayIndex = new Date(year, month, 1).getDay(); 
    const paddingDays = firstDayIndex === 0 ? 6 : firstDayIndex - 1;
    const daysInMonth = new Date(year, month + 1, 0).getDate();

    const grid: (CalendarDay | null)[] = [];

    for (let i = 0; i < paddingDays; i++) {
      grid.push(null);
    }

    const todayStr = this.formatDate(new Date());
    const occupied = this.occupiedDates();

    for (let i = 1; i <= daysInMonth; i++) {
      const currentFullDate = new Date(year, month, i);
      const dateStr = this.formatDate(currentFullDate);
      const isPastDay = currentFullDate < new Date(new Date().setHours(0,0,0,0));

      grid.push({
        date: i,
        fullDateString: dateStr,
        isOccupied: occupied.includes(dateStr),
        isToday: dateStr === todayStr,
        isPast: isPastDay
      });
    }
    return grid;
  });

  public ngOnInit(): void {
    this.fetchOccupiedDates();
  }

  public prevMonth(): void {
    const d = this.currentDate();
    this.currentDate.set(new Date(d.getFullYear(), d.getMonth() - 1, 1));
    this.fetchOccupiedDates();
  }

  public nextMonth(): void {
    const d = this.currentDate();
    this.currentDate.set(new Date(d.getFullYear(), d.getMonth() + 1, 1));
    this.fetchOccupiedDates();
  }

  public selectDate(day: CalendarDay | null): void {
    if (!day) return;
    this.dateSelected.emit(day.fullDateString);
  }

  private fetchOccupiedDates(): void {
    const year = this.currentYear();
    const month = this.currentDate().getMonth() + 1; 

    this.eventService.getOccupiedDates(year, month).subscribe({
      next: (dates) => this.occupiedDates.set(dates),
      error: () => this.occupiedDates.set([])
    });
  }

  private formatDate(d: Date): string {
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}