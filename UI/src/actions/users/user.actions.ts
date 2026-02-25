import { promiApi } from '@/config/api/promiApi';
import { mapUser } from '@/infrastructure/mappers/api.mappers';
import { User } from '@/infrastructure/interfaces';

export async function getUsers(): Promise<User[]> {
  const { data } = await promiApi.get<{ isSuccess: boolean; value: any[] }>('/users');
  return data.value.map(mapUser);
}
