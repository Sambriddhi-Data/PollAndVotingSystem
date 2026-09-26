export interface VoteStatus {
  hasVoted: boolean;
  votedOnBehalfByDelegate: boolean;
  candidateId?: number;
  timestamp?: string;
}

export interface CandidateResult {
  candidateId: number;
  candidateName: string;
  voteCount: number;
}

export interface ElectionResults {
  electionId: number;
  electionStatus: string;
  totalVotesCast: number;
  results: CandidateResult[];
}