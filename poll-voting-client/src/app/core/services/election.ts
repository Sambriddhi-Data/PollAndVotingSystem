import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Election, ElectionRequest } from '../models/election.model';

@Service()
export class ElectionService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/elections`;

  getAll(): Promise<Election[]> {
    return firstValueFrom(this.http.get<Election[]>(this.baseUrl));
  }

  getById(id: number): Promise<Election> {
    return firstValueFrom(this.http.get<Election>(`${this.baseUrl}/${id}`));
  }

  create(request: ElectionRequest): Promise<Election> {
    return firstValueFrom(this.http.post<Election>(this.baseUrl, request));
  }

  update(id: number, request: ElectionRequest): Promise<Election> {
    return firstValueFrom(this.http.put<Election>(`${this.baseUrl}/${id}`, request));
  }

  activate(id: number): Promise<Election> {
    return firstValueFrom(this.http.put<Election>(`${this.baseUrl}/${id}/activate`, {}));
  }

  close(id: number): Promise<Election> {
    return firstValueFrom(this.http.put<Election>(`${this.baseUrl}/${id}/close`, {}));
  }
}