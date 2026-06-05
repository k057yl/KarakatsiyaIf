import { Component, inject, OnInit, OnDestroy, signal, ElementRef, ViewChild, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { EventService } from '../../../features/events/services/event.service';
import { Subscription, interval } from 'rxjs';
import { ASSET_CONSTANTS } from '../../../core/constants/asset-constants';

export interface VipEvent {
  id: string;
  title: string;
  city: string;
  locationName: string;
  startDate: string;
  imageUrl: string | null;
}

@Component({
  selector: 'app-vip-carousel',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  templateUrl: './vip-carousel.component.html',
  styleUrls: ['./vip-carousel.component.scss']
})
export class VipCarouselComponent implements OnInit, OnDestroy {
  private eventService = inject(EventService);
  private platformId = inject(PLATFORM_ID);
  private autoPlaySub?: Subscription;

  @ViewChild('carouselTrack') carouselTrack!: ElementRef<HTMLDivElement>;

  public vipEvents = signal<VipEvent[]>([]);
  public currentIndex = signal<number>(0);

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.loadVipEvents();
    }
  }

  ngOnDestroy(): void {
    this.stopAutoPlay();
  }

  private loadVipEvents(): void {
    this.eventService.getApprovedEvents().subscribe({
      next: (events: any[]) => {
        const vips = events.filter(e => e.isVip);
        const shuffled = this.shuffleArray(vips).slice(0, 5);
        const mappedVips = shuffled.map(e => ({
          id: e.id,
          title: e.title,
          city: e.city,
          locationName: e.locationName,
          startDate: e.startDate,
          imageUrl: e.mainPhotoUrl ? ASSET_CONSTANTS.getEventImage(e.mainPhotoUrl) : null
        }));

        this.vipEvents.set(mappedVips);
        
        if (mappedVips.length > 1) {
          this.startAutoPlay();
        }
      },
      error: (err) => console.error('Ошибка загрузки VIP-карусели', err)
    });
  }

  private startAutoPlay(): void {
    this.stopAutoPlay();
    this.autoPlaySub = interval(4000).subscribe(() => {
      this.nextSlide();
    });
  }

  private stopAutoPlay(): void {
    if (this.autoPlaySub) {
      this.autoPlaySub.unsubscribe();
    }
  }

  public nextSlide(): void {
    if (this.vipEvents().length === 0) return;
    const next = (this.currentIndex() + 1) % this.vipEvents().length;
    this.currentIndex.set(next);
    this.scrollToIndex(next);
  }

  public prevSlide(): void {
    if (this.vipEvents().length === 0) return;
    const prev = (this.currentIndex() - 1 + this.vipEvents().length) % this.vipEvents().length;
    this.currentIndex.set(prev);
    this.scrollToIndex(prev);
  }

  public setSlide(index: number): void {
    this.currentIndex.set(index);
    this.scrollToIndex(index);
    this.startAutoPlay();
  }

  private scrollToIndex(index: number): void {
    if (!this.carouselTrack) return;
    const track = this.carouselTrack.nativeElement;
    const slideWidth = track.clientWidth;
    track.scrollTo({
      left: slideWidth * index,
      behavior: 'smooth'
    });
  }

  private shuffleArray(array: any[]): any[] {
    const arr = [...array];
    for (let i = arr.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [arr[i], arr[j]] = [arr[j], arr[i]];
    }
    return arr;
  }
}