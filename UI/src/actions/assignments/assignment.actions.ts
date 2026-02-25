import { promiApi } from '@/config/api/promiApi';
import { mapAssignment } from '@/infrastructure/mappers/api.mappers';
import { PointAssignment } from '@/infrastructure/interfaces';

export async function getReceivedAssignments(): Promise<PointAssignment[]> {
  const { data } = await promiApi.get<{ isSuccess: boolean; value: any[] }>('/assignments/received');
  return data.value.map(mapAssignment);
}

export async function getAllAssignments(): Promise<PointAssignment[]> {
  const { data } = await promiApi.get<{ isSuccess: boolean; value: any[] }>('/assignments');
  return data.value.map(mapAssignment);
}

export async function createAssignment(dto: {
  toUserId: number;
  points: number;
  category: string;
  message?: string;
}): Promise<PointAssignment> {
  const { data } = await promiApi.post<{ isSuccess: boolean; value: any }>('/assignments', dto);
  return mapAssignment(data.value);
}
