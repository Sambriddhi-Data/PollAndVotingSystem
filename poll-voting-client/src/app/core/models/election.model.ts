export interface Election {
  id: number;
  title: string;
  description: string;
  departmentId: number;
  departmentName: string;
  startDate: string;
  endDate: string;
  status: 'Draft' | 'Active' | 'Closed';
  minTenureYears: number;
  isLocked: boolean;
  createdByUserId: number;
}

export interface ElectionRequest {
  title: string;
  description: string;
  departmentId: number;
  startDate: string;
  endDate: string;
  minTenureYears: number;
}