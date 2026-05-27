import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../../core/services/admin.service';

@Component({
  selector: 'app-reported-comments',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reported-comments.component.html',
  styleUrls: ['./reported-comments.component.scss']
})
export class ReportedCommentsComponent implements OnInit {
  private adminService = inject(AdminService);

  public reports = signal<any[]>([]);
  public isLoading = signal<boolean>(true);

  ngOnInit(): void {
    this.loadReports();
  }

  private loadReports(): void {
    this.adminService.getReportedComments().subscribe({
      next: (data) => {
        this.reports.set(data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  public dismiss(commentId: string): void {
    this.adminService.dismissReport(commentId).subscribe({
      next: () => {
        this.reports.update(list => list.filter(r => r.commentId !== commentId));
      }
    });
  }

  public kill(commentId: string): void {
    if (!window.confirm('Стереть этот коммент из истории вселенной?')) return;

    this.adminService.deleteCommentByReport(commentId).subscribe({
      next: () => {
        this.reports.update(list => list.filter(r => r.commentId !== commentId));
      }
    });
  }
}