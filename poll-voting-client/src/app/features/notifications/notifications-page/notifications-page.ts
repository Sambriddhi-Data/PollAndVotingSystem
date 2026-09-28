import { Component, inject, signal, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { NotificationService } from '../../../core/services/notification';
import { AppNotification } from '../../../core/models/notification.model';

@Component({
  selector: 'app-notifications-page',
  imports: [DatePipe],
  templateUrl: './notifications-page.html',
  styleUrl: './notifications-page.scss'
})
export class NotificationsPage implements OnInit {
  private notificationService = inject(NotificationService);

  notifications = signal<AppNotification[]>([]);
  isLoading = signal(true);

  async ngOnInit(): Promise<void> {
    this.notifications.set(await this.notificationService.getMine());
    this.isLoading.set(false);
  }

  async markRead(id: number): Promise<void> {
    await this.notificationService.markAsRead(id);
    this.notifications.update(list =>
      list.map(n => (n.id === id ? { ...n, isRead: true } : n))
    );
  }
}