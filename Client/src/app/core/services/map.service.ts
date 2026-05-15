import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class MapService {
  private http = inject(HttpClient);
  private nominatimUrl = 'https://nominatim.openstreetmap.org';

  searchLocation(query: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.nominatimUrl}/search`, {
      params: { q: query, format: 'json', addressdetails: '1', limit: '5', 'accept-language': 'uk,ru,en' }
    });
  }

  reverseGeocode(lat: number, lon: number): Observable<any> {
    return this.http.get<any>(`${this.nominatimUrl}/reverse`, {
      params: { lat: lat.toString(), lon: lon.toString(), format: 'json', addressdetails: '1', 'accept-language': 'uk,ru,en' }
    });
  }
}