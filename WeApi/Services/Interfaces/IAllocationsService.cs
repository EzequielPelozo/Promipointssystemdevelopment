using Services.Dtos.Allocations;
using Shared.ResultPattern;

namespace Services.Interfaces;

public interface IAllocationsService
{
    Task<Result<AllocationDto>> GetMyAllocationAsync(int userId, string month);
}
