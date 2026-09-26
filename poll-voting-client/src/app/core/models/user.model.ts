export interface AuthResponse {
  userId: number;
  name: string;
  email: string;
  role: 'Voter' | 'Admin';
  departmentId: number;
  token: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  departmentId: number;
  dateJoined: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}