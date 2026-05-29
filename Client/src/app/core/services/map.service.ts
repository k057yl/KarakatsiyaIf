import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class MapService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/geo/reverse`;

  searchLocation(query: string): Observable<any[]> {
    return this.http.get<any[]>(`https://nominatim.openstreetmap.org/search`, {
      params: { q: query, format: 'json', addressdetails: '1', limit: '5', 'accept-language': 'uk' }
    });
  }

  reverseGeocode(lat: number, lon: number): Observable<any> {
    return this.http.get<any>(this.apiUrl, {
      params: { lat: lat.toString(), lon: lon.toString() }
    });
  }
}