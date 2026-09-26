import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DelegationStatus } from '../models/delegation.model';

@Service()
export class DelegationService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/delegations`;

  create(electionId: number, delegateId: number): Promise<DelegationStatus> {
    return firstValueFrom(
      this.http.post<DelegationStatus>(`${this.baseUrl}/election/${electionId}`, { delegateId })
    );
  }

  revoke(electionId: number): Promise<void> {
    return firstValueFrom(this.http.delete<void>(`${this.baseUrl}/election/${electionId}`));
  }

  getMyStatus(electionId: number): Promise<DelegationStatus> {
    return firstValueFrom(this.http.get<DelegationStatus>(`${this.baseUrl}/election/${electionId}/status`));
  }
}