export interface AppNotification {
  id: number;
  electionId: number;
  electionTitle: string;
  type: string;
  message: string;
  isRead: boolean;
  createdAt: string;
}