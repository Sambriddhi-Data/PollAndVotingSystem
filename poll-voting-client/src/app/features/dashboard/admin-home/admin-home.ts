import { Component, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { RouterLink } from '@angular/router';
import * as signalR from '@microsoft/signalr';
import { ElectionService } from '../../../core/services/election';
import { VoteService } from '../../../core/services/vote';
import { CurrentUser } from '../../../core/services/current-user';
import { Election } from '../../../core/models/election.model';
import { ElectionResults } from '../../../core/models/vote.model';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-admin-home',
  imports: [RouterLink],
  templateUrl: './admin-home.html',
  styleUrl: './admin-home.scss'
})
export class AdminHome implements OnInit, OnDestroy {
  private electionService = inject(ElectionService);
  private voteService = inject(VoteService);
  currentUser = inject(CurrentUser);

  auth = this.currentUser.auth;
  activeElections = signal<Election[]>([]);
  resultsMap = signal<Record<number, ElectionResults>>({});
  isLoading = signal(true);

  private hubConnection: signalR.HubConnection | null = null;

  async ngOnInit(): Promise<void> {
    const all = await this.electionService.getAll();
    const active = all.filter(e => e.status === 'Active');
    this.activeElections.set(active);

    for (const e of active) {
      try {
        const results = await this.voteService.getResults(e.id);
        this.resultsMap.update(m => ({ ...m, [e.id]: results }));
      } catch { /* no votes yet */ }
    }

    this.isLoading.set(false);

    if (active.length > 0) {
      await this.connectLiveTally(active.map(e => e.id));
    }
  }

  ngOnDestroy(): void {
    this.hubConnection?.stop();
  }

  private async connectLiveTally(electionIds: number[]): Promise<void> {
    const token = this.currentUser.getToken();

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl.replace('/api', '')}/hubs/election-results`, {
        accessTokenFactory: () => token ?? ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ResultsUpdated', (data: ElectionResults) => {
      this.resultsMap.update(m => ({ ...m, [data.electionId]: data }));
    });

    this.hubConnection.on('ElectionClosed', (data: { electionId: number }) => {
      this.activeElections.update(list => list.filter(e => e.id !== data.electionId));
    });

    await this.hubConnection.start();
    for (const id of electionIds) {
      await this.hubConnection.invoke('JoinElectionGroup', id);
    }
  }
}