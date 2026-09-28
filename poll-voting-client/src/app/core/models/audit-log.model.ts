export interface AuditLogEntry {
  id: number;
  userId: number;
  userName: string;
  action: string;
  entityType: string;
  entityId: number;
  timestamp: string;
  details?: string;
}