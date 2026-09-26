import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ElectionResults, VoteStatus } from '../models/vote.model';

@Service()
export class VoteService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/votes`;

  castVote(electionId: number, candidateId: number): Promise<{ message: string }> {
    return firstValueFrom(
      this.http.post<{ message: string }>(`${this.baseUrl}/election/${electionId}`, { candidateId })
    );
  }

  getMyStatus(electionId: number): Promise<VoteStatus> {
    return firstValueFrom(this.http.get<VoteStatus>(`${this.baseUrl}/election/${electionId}/status`));
  }

  getResults(electionId: number): Promise<ElectionResults> {
    return firstValueFrom(this.http.get<ElectionResults>(`${this.baseUrl}/election/${electionId}/results`));
  }
}