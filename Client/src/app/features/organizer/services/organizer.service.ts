import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Observable } from 'rxjs';

export interface ApplyForOrganizerDto {
  name: string;
  phone: string;
  email: string;
  website?: string;
  telegram?: string;
  instagram?: string;
}

@Injectable({ providedIn: 'root' })
export class OrganizerService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/users/me`;

  apply(data: ApplyForOrganizerDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/apply-organizer`, data);
  }
}