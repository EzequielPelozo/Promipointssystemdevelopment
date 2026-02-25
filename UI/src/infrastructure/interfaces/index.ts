export interface User {
  id: string;
  name: string;
  email: string;
  role: 'employee' | 'people';
  department: string;
}

export interface MonthlyAllocation {
  userId: string;
  month: string;
  pointsRemaining: number;
  pointsReceived: number;
}

export interface PointAssignment {
  id: string;
  fromUserId: string;
  toUserId: string;
  points: number;
  category: string;
  message?: string;
  timestamp: number;
  month: string;
}

export type Category =
  | 'Trabajo en equipo'
  | 'Innovación'
  | 'Liderazgo'
  | 'Colaboración'
  | 'Compromiso'
  | 'Excelencia'
  | 'Actitud positiva'
  | 'Comunicación efectiva';
