import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AppNotification } from '../models/notification.model';

@Service()
export class NotificationService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/notifications`;

  getMine(): Promise<AppNotification[]> {
    return firstValueFrom(this.http.get<AppNotification[]>(`${this.baseUrl}/mine`));
  }

  markAsRead(id: number): Promise<void> {
    return firstValueFrom(this.http.put<void>(`${this.baseUrl}/${id}/read`, {}));
  }
}