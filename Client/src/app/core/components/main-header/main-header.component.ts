import { Component, inject, Input, Output, EventEmitter, HostListener, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../../../features/auth/services/auth.service';

@Component({
  selector: 'app-main-header',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, TranslateModule],
  templateUrl: './main-header.component.html',
  styleUrls: ['./main-header.component.scss']
})
export class MainHeaderComponent implements OnInit {
  public authService = inject(AuthService);

  @Input() currentTheme = 'dark';
  @Input() currentLang = 'uk';
  @Output() languageChanged = new EventEmitter<string>();
  @Output() themeToggled = new EventEmitter<void>();

  public isLangMenuOpen = false;

  ngOnInit(): void {
  }

  public toggleLangMenu(event: Event): void {
    event.stopPropagation();
    this.isLangMenuOpen = !this.isLangMenuOpen;
  }

  public selectLang(lang: string): void {
    this.languageChanged.emit(lang);
    this.isLangMenuOpen = false;
  }

  public logout(): void {
    this.authService.logout();
  }

  @HostListener('document:click', ['$event'])
  public closeLangMenu(event: Event): void {
    this.isLangMenuOpen = false;
  }
}