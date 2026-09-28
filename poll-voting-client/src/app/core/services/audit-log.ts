import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuditLogEntry } from '../models/audit-log.model';

@Service()
export class AuditLogService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/auditlogs`;

  getAll(entityType?: string, entityId?: number): Promise<AuditLogEntry[]> {
    const params: string[] = [];
    if (entityType) params.push(`entityType=${entityType}`);
    if (entityId) params.push(`entityId=${entityId}`);
    const query = params.length ? `?${params.join('&')}` : '';
    return firstValueFrom(this.http.get<AuditLogEntry[]>(`${this.baseUrl}${query}`));
  }
}