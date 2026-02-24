using Business.Entities;
using Services.Dtos.Allocations;

namespace Services.Mappers;

public static class AllocationMapper
{
    public static AllocationDto ToDto(this MonthlyAllocation allocation) =>
        new(allocation.UserId, allocation.Month, allocation.PointsRemaining, allocation.PointsReceived);
}
