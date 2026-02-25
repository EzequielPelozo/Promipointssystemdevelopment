import { promiApi } from '@/config/api/promiApi';
import { mapAllocation } from '@/infrastructure/mappers/api.mappers';
import { MonthlyAllocation } from '@/infrastructure/interfaces';

export async function getMyAllocation(month: string): Promise<MonthlyAllocation> {
  const { data } = await promiApi.get<{ isSuccess: boolean; value: any }>(`/allocations/me/${month}`);
  return mapAllocation(data.value);
}
