import { promiApi } from '@/config/api/promiApi';
import { mapUser } from '@/infrastructure/mappers/api.mappers';
import { User } from '@/infrastructure/interfaces';

const TOKEN_KEY = 'promipoints_token';

export async function login(email: string, password: string): Promise<{ user: User; token: string }> {
  const { data } = await promiApi.post<{ isSuccess: boolean; value: { token: string; user: any } }>('/auth/login', { email, password });
  const token = data.value.token;
  localStorage.setItem(TOKEN_KEY, token);
  return { user: mapUser(data.value.user), token };
}

export async function getMe(): Promise<User> {
  const { data } = await promiApi.get<{ isSuccess: boolean; value: any }>('/auth/me');
  return mapUser(data.value);
}

export function logout(): void {
  localStorage.removeItem(TOKEN_KEY);
}

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}
