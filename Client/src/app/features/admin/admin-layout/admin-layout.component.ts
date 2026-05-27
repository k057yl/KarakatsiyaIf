import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  template: `
    <div class="admin-layout">
      <h2>Панель Модерации 👑</h2>
      
      <div class="admin-tabs">
        <a routerLink="/admin/organizers" routerLinkActive="active" class="tab-link">Заявки Организаторов</a>
        <a routerLink="/admin/events" routerLinkActive="active" class="tab-link">Модерация Ивентов</a>
        <a routerLink="/admin/active-events" routerLinkActive="active" class="tab-link">⚡ Активные события</a>
        <a routerLink="/admin/reported-comments" routerLinkActive="active" class="tab-link">🚩 Жалобы на комменты</a>
      </div>

      <div class="admin-content">
        <router-outlet></router-outlet>
      </div>
    </div>
  `,
  styles: [`
    .admin-layout { max-width: 1200px; margin: 0 auto; }
    .admin-tabs { display: flex; gap: 15px; margin-top: 20px; margin-bottom: 30px; border-bottom: 1px solid var(--border-color); padding-bottom: 10px; flex-wrap: wrap; }
    .tab-link { padding: 8px 16px; text-decoration: none; color: var(--text-muted); border-radius: 6px; font-weight: 500; transition: all 0.2s;
      &:hover { background: var(--input-bg); }
      &.active { background: var(--primary-color); color: #fff; }
    }
  `]
})
export class AdminLayoutComponent {}