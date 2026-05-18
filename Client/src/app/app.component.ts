import { Component, inject, OnInit, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from './core/services/auth.service';
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
  private readonly platformId = inject(PLATFORM_ID);
  public readonly authService = inject(AuthService);
  private readonly translateService = inject(TranslateService);

  public ngOnInit(): void {
    this.initializeLocalization();
  }

  public switchLanguage(languageCode: string): void {
    this.translateService.use(languageCode);
    
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(this.LANGUAGE_STORAGE_KEY, languageCode);
    }
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
}