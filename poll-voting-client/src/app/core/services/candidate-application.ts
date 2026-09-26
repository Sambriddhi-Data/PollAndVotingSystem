import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Candidate, CandidateApplication } from '../models/candidate.model';

@Service()
export class CandidateApplicationService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/candidateapplications`;

  apply(electionId: number, statement: string): Promise<CandidateApplication> {
    return firstValueFrom(
      this.http.post<CandidateApplication>(`${this.baseUrl}/election/${electionId}/apply`, { statement })
    );
  }

  getByElection(electionId: number): Promise<CandidateApplication[]> {
    return firstValueFrom(
      this.http.get<CandidateApplication[]>(`${this.baseUrl}/election/${electionId}`)
    );
  }

  getMyApplications(): Promise<CandidateApplication[]> {
    return firstValueFrom(this.http.get<CandidateApplication[]>(`${this.baseUrl}/mine`));
  }

  approve(applicationId: number): Promise<CandidateApplication> {
    return firstValueFrom(this.http.put<CandidateApplication>(`${this.baseUrl}/${applicationId}/approve`, {}));
  }

  reject(applicationId: number, rejectionReason?: string): Promise<CandidateApplication> {
    return firstValueFrom(
      this.http.put<CandidateApplication>(`${this.baseUrl}/${applicationId}/reject`, { rejectionReason })
    );
  }

  getCandidates(electionId: number): Promise<Candidate[]> {
    return firstValueFrom(
      this.http.get<Candidate[]>(`${this.baseUrl}/election/${electionId}/candidates`)
    );
  }
}