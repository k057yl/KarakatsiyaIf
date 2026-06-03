import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CreateCommentDto } from '../dtos/comment.dto';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/comments`;

  public createComment(payload: CreateCommentDto): Observable<{ commentId: string, message: string }> {
    return this.http.post<{ commentId: string, message: string }>(this.apiUrl, payload);
  }

  public reportComment(commentId: string, reason: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${commentId}/report`, { reason });
  }
}