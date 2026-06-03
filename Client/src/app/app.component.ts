import { Component, inject, OnInit, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from './features/auth/services/auth.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    TranslateModule
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  private readonly LANGUAGE_STORAGE_KEY = 'lang';
  private readonly THEME_STORAGE_KEY = 'theme';
  private readonly platformId = inject(PLATFORM_ID);
  
  public readonly authService = inject(AuthService);
  private readonly translateService = inject(TranslateService);
  
  public currentTheme = 'dark';

  public ngOnInit(): void {
    this.initializeLocalization();
    this.initializeTheme();
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