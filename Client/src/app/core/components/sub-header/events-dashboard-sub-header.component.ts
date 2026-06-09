import { Component, inject, OnInit, OnDestroy, PLATFORM_ID, signal, Output, EventEmitter, Input } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { VipCarouselComponent } from '../../../shared/ui/vip-carousel/vip-carousel.component';
import { AuthService } from '../../../features/auth/services/auth.service';
import { interval, Subscription } from 'rxjs';

@Component({
  selector: 'app-events-dashboard-sub-header',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule, VipCarouselComponent],
  templateUrl: './events-dashboard-sub-header.component.html',
  styleUrls: ['./events-dashboard-sub-header.component.scss']
})
export class EventsDashboardSubHeaderComponent implements OnInit, OnDestroy {
  public authService = inject(AuthService);
  private platformId = inject(PLATFORM_ID);
  private rotationSub?: Subscription;

  @Input() currentSort: 'date' | 'location' = 'date';
  @Input() categoriesList: any[] = [];

  @Output() sortChanged = new EventEmitter<'date' | 'location'>();
  @Output() categoryChanged = new EventEmitter<string>();
  @Output() locationChanged = new EventEmitter<string>();

  public currentSlideIndex = signal<number>(0);
  public selectedCategoryId = signal<string>('');
  public searchLocationQuery = signal<string>('');

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.rotationSub = interval(12000).subscribe(() => {
        this.currentSlideIndex.update(idx => (idx === 0 ? 1 : 0));
      });
    }
  }

  ngOnDestroy(): void {
    if (this.rotationSub) {
      this.rotationSub.unsubscribe();
    }
  }

  public changeSort(criteria: 'date' | 'location'): void {
    this.sortChanged.emit(criteria);
  }

  public onCategoryChange(): void {
    this.categoryChanged.emit(this.selectedCategoryId());
  }

  public onLocationChange(): void {
    this.locationChanged.emit(this.searchLocationQuery());
  }
}