import axios from 'axios';
import { User, MonthlyAllocation, PointAssignment } from '../types';

const TOKEN_KEY = 'promipoints_token';

const client = axios.create({
  // baseURL: '/api',
  baseURL: '',
});

client.interceptors.request.use((config) => {
  const token = localStorage.getItem(TOKEN_KEY);
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Auth
export async function login(email: string, password: string): Promise<{ user: User; token: string }> {
  const { data } = await client.post<{ isSuccess: boolean; value: { token: string; user: ApiUser } }>('/auth/login', { email, password });
  const apiUser = data.value.user;
  const token = data.value.token;
  localStorage.setItem(TOKEN_KEY, token);
  return { user: mapUser(apiUser), token };
}

export async function getMe(): Promise<User> {
  const { data } = await client.get<{ isSuccess: boolean; value: ApiUser }>('/auth/me');
  return mapUser(data.value);
}

export function logout(): void {
  localStorage.removeItem(TOKEN_KEY);
}

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

// Users
export async function getUsers(): Promise<User[]> {
  const { data } = await client.get<{ isSuccess: boolean; value: ApiUser[] }>('/users');
  return data.value.map(mapUser);
}

// Allocations
export async function getMyAllocation(month: string): Promise<MonthlyAllocation> {
  const { data } = await client.get<{ isSuccess: boolean; value: ApiAllocation }>(`/allocations/me/${month}`);
  return mapAllocation(data.value);
}

// Assignments
export async function getReceivedAssignments(): Promise<PointAssignment[]> {
  const { data } = await client.get<{ isSuccess: boolean; value: ApiAssignment[] }>('/assignments/received');
  return data.value.map(mapAssignment);
}

export async function getAllAssignments(): Promise<PointAssignment[]> {
  const { data } = await client.get<{ isSuccess: boolean; value: ApiAssignment[] }>('/assignments');
  return data.value.map(mapAssignment);
}

export async function createAssignment(dto: { toUserId: number; points: number; category: string; message?: string }): Promise<PointAssignment> {
  const { data } = await client.post<{ isSuccess: boolean; value: ApiAssignment }>('/assignments', dto);
  return mapAssignment(data.value);
}

// API response shapes (backend uses int IDs, frontend uses string IDs)
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

function mapUser(u: ApiUser): User {
  return { id: String(u.id), name: u.name, email: u.email, role: u.role, department: u.department };
}

function mapAllocation(a: ApiAllocation): MonthlyAllocation {
  return { userId: String(a.userId), month: a.month, pointsRemaining: a.pointsRemaining, pointsReceived: a.pointsReceived };
}

function mapAssignment(a: ApiAssignment): PointAssignment {
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
