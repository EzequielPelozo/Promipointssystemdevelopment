using DataAccess.Interfaces;
using Serilog;
using Services.Dtos.Allocations;
using Services.Interfaces;
using Services.Mappers;
using Shared.ResultPattern;

namespace Services.Implementations;

public class AllocationsService(IUnitOfWork unitOfWork) : IAllocationsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<AllocationDto>> GetMyAllocationAsync(int userId, string month)
    {
        try
        {
            var allocation = await _unitOfWork.Allocations.GetByUserAndMonth(userId, month);

            if (allocation is null)
            {
                // Create allocation on demand if it doesn't exist
                allocation = new Business.Entities.MonthlyAllocation
                {
                    UserId = userId,
                    Month = month,
                    PointsRemaining = 10,
                    PointsReceived = 0
                };
                await _unitOfWork.Allocations.Add(allocation);
                await _unitOfWork.SaveChangesAsync();
            }

            return allocation.ToDto();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching allocation for user {UserId} month {Month}", userId, month);
            return Error.InternalServerError("Error al obtener la asignación mensual");
        }
    }
}
