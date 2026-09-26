import { Component, inject, signal, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ElectionService } from '../../../core/services/election';
import { CurrentUser } from '../../../core/services/current-user';
import { Election } from '../../../core/models/election.model';
import { DatePipe } from '@angular/common';

@Component({
  imports: [RouterLink,DatePipe],
  selector: 'app-election-list',
  styleUrl: './election-list.scss',
  templateUrl: './election-list.html'
})
export class ElectionList implements OnInit {
  private electionService = inject(ElectionService);
  currentUser = inject(CurrentUser);

  elections = signal<Election[]>([]);
  isLoading = signal(true);
  errorMessage = signal('');

  async ngOnInit(): Promise<void> {
    try {
      const data = await this.electionService.getAll();
      this.elections.set(data);
    } catch (err: any) {
      this.errorMessage.set(err.error?.message ?? 'Failed to load elections.');
    } finally {
      this.isLoading.set(false);
    }
  }
}