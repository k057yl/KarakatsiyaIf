import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = `${environment.apiUrl}/payments`;

  public payForVip(eventId: string): void {
    this.http.post<any>(`${this.apiUrl}/checkout/${eventId}`, {}).subscribe(data => {
      
      if (data.useMock) {
        const confirmPay = window.confirm('💰 [LOCAL DEV MODE] Симулировать успешную оплату 500 грн?');
        if (confirmPay) {
          this.http.post<any>(`${this.apiUrl}/fake-success/${eventId}`, {}).subscribe(() => {
            alert('VIP статус успешно получен (Симуляция)!');
            this.router.navigate(['/organizer/dashboard']);
            window.location.reload();
          });
        }
        return;
      }

      const form = document.createElement('form');
      form.method = 'POST';
      form.action = 'https://secure.wayforpay.com/pay';
      form.style.display = 'none';

      Object.keys(data).forEach(key => {
        if (key === 'useMock') return;
        
        if (Array.isArray(data[key])) {
          data[key].forEach((val: string) => {
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = `${key}[]`;
            input.value = val;
            form.appendChild(input);
          });
        } else {
          const input = document.createElement('input');
          input.type = 'hidden';
          input.name = key;
          input.value = data[key];
          form.appendChild(input);
        }
      });

      document.body.appendChild(form);
      form.submit();
    });
  }
}