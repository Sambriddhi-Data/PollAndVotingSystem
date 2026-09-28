import { Component, inject, signal, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { ElectionService } from '../../../core/services/election';
import { CurrentUser } from '../../../core/services/current-user';
import { Election } from '../../../core/models/election.model';

@Component({
  selector: 'app-voter-home',
  imports: [RouterLink, DatePipe],
  templateUrl: './voter-home.html',
  styleUrl: './voter-home.scss'
})
export class VoterHome implements OnInit {
  private electionService = inject(ElectionService);
  currentUser = inject(CurrentUser);

  auth = this.currentUser.auth;
  elections = signal<Election[]>([]);
  isLoading = signal(true);

  async ngOnInit(): Promise<void> {
    this.elections.set(await this.electionService.getAll());
    this.isLoading.set(false);
  }
}