export interface CandidateApplication {
  id: number;
  electionId: number;
  userId: number;
  userName: string;
  statement: string;
  status: 'Pending' | 'Approved' | 'Rejected';
  reviewedByUserId?: number;
  reviewedAt?: string;
  rejectionReason?: string;
}

export interface Candidate {
  id: number;
  electionId: number;
  userId: number;
  userName: string;
  statement: string;
}