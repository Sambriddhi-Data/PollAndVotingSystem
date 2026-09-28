import { Component, inject, signal, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { AuditLogService } from '../../../core/services/audit-log';
import { AuditLogEntry } from '../../../core/models/audit-log.model';
import { RouterLink } from '@angular/router';

@Component({
  imports: [DatePipe, RouterLink],
  selector: 'app-audit-log-page',
  templateUrl: './audit-log-page.html',
  styleUrl: './audit-log-page.scss'
})
export class AuditLogPage implements OnInit {
  private auditLogService = inject(AuditLogService);

  logs = signal<AuditLogEntry[]>([]);
  isLoading = signal(true);

  async ngOnInit(): Promise<void> {
    this.logs.set(await this.auditLogService.getAll());
    this.isLoading.set(false);
  }
}