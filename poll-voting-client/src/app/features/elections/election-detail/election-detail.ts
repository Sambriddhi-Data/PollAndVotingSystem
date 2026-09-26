import { Component, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import * as signalR from '@microsoft/signalr';
import { ElectionService } from '../../../core/services/election';
import { CandidateApplicationService } from '../../../core/services/candidate-application';
import { VoteService } from '../../../core/services/vote';
import { DelegationService } from '../../../core/services/delegation';
import { CurrentUser } from '../../../core/services/current-user';
import { Election } from '../../../core/models/election.model';
import { Candidate, CandidateApplication } from '../../../core/models/candidate.model';
import { VoteStatus, ElectionResults } from '../../../core/models/vote.model';
import { DelegationStatus } from '../../../core/models/delegation.model';
import { environment } from '../../../../environments/environment';

@Component({
  imports: [RouterLink],
  selector: 'app-election-detail',
  styleUrl: './election-detail.scss',
  templateUrl: './election-detail.html'
})
export class ElectionDetail implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private electionService = inject(ElectionService);
  private applicationService = inject(CandidateApplicationService);
  private voteService = inject(VoteService);
  private delegationService = inject(DelegationService);
  currentUser = inject(CurrentUser);

  electionId = Number(this.route.snapshot.paramMap.get('id'));

  election = signal<Election | null>(null);
  candidates = signal<Candidate[]>([]);
  pendingApplications = signal<CandidateApplication[]>([]);
  voteStatus = signal<VoteStatus | null>(null);
  delegationStatus = signal<DelegationStatus | null>(null);
  results = signal<ElectionResults | null>(null);

  applyStatement = signal('');
  selectedCandidateId = signal<number | null>(null);
  selectedDelegateId = signal<number | null>(null);

  errorMessage = signal('');
  actionMessage = signal('');

  private hubConnection: signalR.HubConnection | null = null;

  async ngOnInit(): Promise<void> {
    await this.loadAll();

    if (this.currentUser.isAdmin() && this.election()?.status === 'Active') {
      this.connectLiveTally();
    }
  }

  ngOnDestroy(): void {
    this.hubConnection?.stop();
  }

  async loadAll(): Promise<void> {
    try {
      const election = await this.electionService.getById(this.electionId);
      this.election.set(election);

      if (election.status === 'Draft' && this.currentUser.isAdmin()) {
        this.pendingApplications.set(await this.applicationService.getByElection(this.electionId));
      }

      if (election.status !== 'Draft') {
        this.candidates.set(await this.applicationService.getCandidates(this.electionId));
      }

      if (election.status === 'Active' && !this.currentUser.isAdmin()) {
        this.voteStatus.set(await this.voteService.getMyStatus(this.electionId));
        this.delegationStatus.set(await this.delegationService.getMyStatus(this.electionId));
      }

      if (election.status === 'Closed' || this.currentUser.isAdmin()) {
        try {
          this.results.set(await this.voteService.getResults(this.electionId));
        } catch {
          // Admin viewing an Active election before any votes exist is fine; ignore.
        }
      }
    } catch (err: any) {
      this.errorMessage.set(err.error?.message ?? 'Failed to load election.');
    }
  }

  async connectLiveTally(): Promise<void> {
    const token = this.currentUser.getToken();

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl.replace('/api', '')}/hubs/election-results`, {
        accessTokenFactory: () => token ?? ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ResultsUpdated', (data: ElectionResults) => {
      this.results.set(data);
    });

    this.hubConnection.on('ElectionClosed', () => {
      this.actionMessage.set('This election has just closed.');
      this.loadAll();
    });

    try {
      await this.hubConnection.start();
      await this.hubConnection.invoke('JoinElectionGroup', this.electionId);
    } catch (err) {
      console.error('SignalR connection failed', err);
    }
  }

  async onApply(): Promise<void> {
    this.errorMessage.set('');
    try {
      await this.applicationService.apply(this.electionId, this.applyStatement());
      this.actionMessage.set('Application submitted.');
      this.applyStatement.set('');
    } catch (err: any) {
      this.errorMessage.set(err.error?.message ?? 'Failed to apply.');
    }
  }

  async onApprove(applicationId: number): Promise<void> {
    await this.applicationService.approve(applicationId);
    await this.loadAll();
  }

  async onReject(applicationId: number): Promise<void> {
    await this.applicationService.reject(applicationId, 'Not approved by admin.');
    await this.loadAll();
  }

  async onActivate(): Promise<void> {
    this.election.set(await this.electionService.activate(this.electionId));
    this.connectLiveTally();
  }

  async onClose(): Promise<void> {
    this.election.set(await this.electionService.close(this.electionId));
  }

  async onVote(): Promise<void> {
    const candidateId = this.selectedCandidateId();
    if (!candidateId) return;

    this.errorMessage.set('');
    try {
      await this.voteService.castVote(this.electionId, candidateId);
      this.actionMessage.set('Vote cast successfully.');
      this.voteStatus.set(await this.voteService.getMyStatus(this.electionId));
    } catch (err: any) {
      this.errorMessage.set(err.error?.message ?? 'Failed to cast vote.');
    }
  }

  async onDelegate(): Promise<void> {
    const delegateId = this.selectedDelegateId();
    if (!delegateId) return;

    this.errorMessage.set('');
    try {
      await this.delegationService.create(this.electionId, delegateId);
      this.actionMessage.set('Vote delegated.');
      this.delegationStatus.set(await this.delegationService.getMyStatus(this.electionId));
    } catch (err: any) {
      this.errorMessage.set(err.error?.message ?? 'Failed to delegate.');
    }
  }

  async onRevokeDelegation(): Promise<void> {
    this.errorMessage.set('');
    try {
      await this.delegationService.revoke(this.electionId);
      this.actionMessage.set('Delegation revoked.');
      this.delegationStatus.set(await this.delegationService.getMyStatus(this.electionId));
    } catch (err: any) {
      this.errorMessage.set(err.error?.message ?? 'Failed to revoke delegation.');
    }
  }
}