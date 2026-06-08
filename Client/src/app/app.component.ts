import { Component, inject, OnInit, PLATFORM_ID, signal } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { MainHeaderComponent } from './core/components/main-header/main-header.component';
import { MainFooterComponent } from './core/components/main-footer/main-footer.component';
import { EventsDashboardSubHeaderComponent } from './core/components/sub-header/events-dashboard-sub-header.component';
import { EventHubComponent } from './features/events/event-hub/event-hub.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    MainHeaderComponent,
    MainFooterComponent,
    EventsDashboardSubHeaderComponent
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  private router = inject(Router);
  private readonly LANGUAGE_STORAGE_KEY = 'lang';
  private readonly THEME_STORAGE_KEY = 'theme';
  private readonly platformId = inject(PLATFORM_ID);
  private readonly translateService = inject(TranslateService);
  
  public isEventsPage = signal<boolean>(false);
  public currentTheme = 'dark';
  public activeHubComponent: EventHubComponent | null = null;

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

  public onRouteActivated(component: any): void {
    if (component instanceof EventHubComponent) {
      this.activeHubComponent = component;
    } else {
      this.activeHubComponent = null;
    }
  }

  public onSortChanged(criteria: 'date' | 'location'): void {
    if (this.activeHubComponent) {
      this.activeHubComponent.onSortExternalChanged(criteria);
    }
  }

  public onLanguageChanged(languageCode: string): void {
    this.translateService.use(languageCode);
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(this.LANGUAGE_STORAGE_KEY, languageCode);
    }
  }

  public onThemeToggled(): void {
    this.currentTheme = this.currentTheme === 'dark' ? 'light' : 'dark';
    this.applyTheme(this.currentTheme);
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