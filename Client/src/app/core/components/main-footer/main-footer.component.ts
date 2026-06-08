import { Component, signal, OnInit, OnDestroy, PLATFORM_ID, inject } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-main-footer',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './main-footer.component.html',
  styleUrls: ['./main-footer.component.scss']
})
export class MainFooterComponent implements OnInit, OnDestroy {
  private platformId = inject(PLATFORM_ID);
  private intervalId: any;

  public activeUsersOnline = signal<number>(142);

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.intervalId = setInterval(() => {
        const randomDelta = Math.floor(Math.random() * 7) - 3;
        this.activeUsersOnline.update(current => Math.max(130, current + randomDelta));
      }, 7000);
    }
  }

  ngOnDestroy(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }
}