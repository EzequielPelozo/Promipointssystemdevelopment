import { User, MonthlyAllocation, PointAssignment } from '../interfaces';

interface ApiUser {
  id: number;
  name: string;
  email: string;
  role: 'employee' | 'people';
  department: string;
}

interface ApiAllocation {
  userId: number;
  month: string;
  pointsRemaining: number;
  pointsReceived: number;
}

interface ApiAssignment {
  id: number;
  fromUserId: number;
  toUserId: number;
  points: number;
  category: string;
  message?: string;
  timestamp: number;
  month: string;
}

export function mapUser(u: ApiUser): User {
  return { id: String(u.id), name: u.name, email: u.email, role: u.role, department: u.department };
}

export function mapAllocation(a: ApiAllocation): MonthlyAllocation {
  return { userId: String(a.userId), month: a.month, pointsRemaining: a.pointsRemaining, pointsReceived: a.pointsReceived };
}

export function mapAssignment(a: ApiAssignment): PointAssignment {
  return {
    id: String(a.id),
    fromUserId: String(a.fromUserId),
    toUserId: String(a.toUserId),
    points: a.points,
    category: a.category,
    message: a.message,
    timestamp: a.timestamp,
    month: a.month,
  };
}
