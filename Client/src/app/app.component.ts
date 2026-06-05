import { Component, inject, OnInit, PLATFORM_ID, signal } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from './features/auth/services/auth.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { VipCarouselComponent } from './shared/ui/vip-carousel/vip-carousel.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    TranslateModule,
    VipCarouselComponent
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  private router = inject(Router);
  private readonly LANGUAGE_STORAGE_KEY = 'lang';
  private readonly THEME_STORAGE_KEY = 'theme';
  private readonly platformId = inject(PLATFORM_ID);
  
  public readonly authService = inject(AuthService);
  private readonly translateService = inject(TranslateService);
  
  public isEventsPage = signal<boolean>(false);

  public currentTheme = 'dark';

  public ngOnInit(): void {
    this.initializeLocalization();
    this.initializeTheme();
    this.trackRoute();
  }

  private trackRoute(): void {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      const url = event.urlAfterRedirects;
      this.isEventsPage.set(url === '/' || url === '/events' || url.startsWith('/events?'));
    });
  }

  public switchLanguage(languageCode: string): void {
    this.translateService.use(languageCode);
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(this.LANGUAGE_STORAGE_KEY, languageCode);
    }
  }

  public toggleTheme(): void {
    this.currentTheme = this.currentTheme === 'dark' ? 'light' : 'dark';
    this.applyTheme(this.currentTheme);
  }

  public logout(): void {
    this.authService.logout();
  }

  private initializeLocalization(): void {
    const fallbackLanguage = 'uk';
    let targetLanguage = fallbackLanguage;

    if (isPlatformBrowser(this.platformId)) {
      const savedLanguage = localStorage.getItem(this.LANGUAGE_STORAGE_KEY);
      if (savedLanguage) {
        targetLanguage = savedLanguage;
      }
    }

    this.translateService.setDefaultLang(fallbackLanguage);
    this.translateService.use(targetLanguage);
  }

  private initializeTheme(): void {
    if (isPlatformBrowser(this.platformId)) {
      const savedTheme = localStorage.getItem(this.THEME_STORAGE_KEY);
      
      if (savedTheme) {
        this.currentTheme = savedTheme;
      } else {
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        this.currentTheme = prefersDark ? 'dark' : 'light';
      }
      
      this.applyTheme(this.currentTheme);
    }
  }

  private applyTheme(theme: string): void {
    if (isPlatformBrowser(this.platformId)) {
      document.documentElement.setAttribute('data-theme', theme);
      localStorage.setItem(this.THEME_STORAGE_KEY, theme);
    }
  }
}