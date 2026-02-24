using DataAccess.Interfaces;
using Serilog;
using Services.Dtos.Assignments;
using Services.Interfaces;
using Services.Mappers;
using Shared.ResultPattern;

namespace Services.Implementations;

public class AssignmentsService(IUnitOfWork unitOfWork) : IAssignmentsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<List<AssignmentDto>>> GetReceivedAssignmentsAsync(int userId)
    {
        try
        {
            var assignments = await _unitOfWork.Assignments.GetByReceiver(userId);
            return assignments.Select(a => new AssignmentDto(
                a.Id, a.FromUserId, string.Empty, a.ToUserId, string.Empty,
                a.Points, a.Category, a.Message, a.Timestamp, a.Month
            )).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching received assignments for user {UserId}", userId);
            return Error.InternalServerError("Error al obtener los reconocimientos");
        }
    }

    public async Task<Result<List<AssignmentDto>>> GetAllAssignmentsAsync()
    {
        try
        {
            var assignments = await _unitOfWork.Assignments.GetAllWithUsers();
            return assignments.Select(a => a.ToDto()).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching all assignments");
            return Error.InternalServerError("Error al obtener los reconocimientos");
        }
    }

    public async Task<Result<AssignmentDto>> CreateAssignmentAsync(int fromUserId, CreateAssignmentDto dto)
    {
        try
        {
            if (fromUserId == dto.ToUserId)
                return Error.ValidationError("No puedes asignarte puntos a ti mismo");

            var month = DateTime.UtcNow.ToString("yyyy-MM");
            var senderAllocation = await _unitOfWork.Allocations.GetByUserAndMonth(fromUserId, month);

            if (senderAllocation is null)
                return Error.RecordNotFound("No se encontró la asignación mensual del remitente");

            if (senderAllocation.PointsRemaining < dto.Points)
                return Error.ValidationError($"Solo tienes {senderAllocation.PointsRemaining} puntos disponibles");

            var assignment = new Business.Entities.PointAssignment
            {
                FromUserId = fromUserId,
                ToUserId = dto.ToUserId,
                Points = dto.Points,
                Category = dto.Category,
                Message = dto.Message,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Month = month
            };

            await _unitOfWork.Assignments.Add(assignment);

            senderAllocation.PointsRemaining -= dto.Points;
            _unitOfWork.Allocations.Update(senderAllocation);

            var receiverAllocation = await _unitOfWork.Allocations.GetByUserAndMonth(dto.ToUserId, month);
            if (receiverAllocation is null)
            {
                receiverAllocation = new Business.Entities.MonthlyAllocation
                {
                    UserId = dto.ToUserId,
                    Month = month,
                    PointsRemaining = 10,
                    PointsReceived = dto.Points
                };
                await _unitOfWork.Allocations.Add(receiverAllocation);
            }
            else
            {
                receiverAllocation.PointsReceived += dto.Points;
                _unitOfWork.Allocations.Update(receiverAllocation);
            }

            await _unitOfWork.SaveChangesAsync();

            Log.Information("Assignment created: {Points} pts from {From} to {To}", dto.Points, fromUserId, dto.ToUserId);

            return new AssignmentDto(
                assignment.Id, assignment.FromUserId, string.Empty,
                assignment.ToUserId, string.Empty,
                assignment.Points, assignment.Category, assignment.Message,
                assignment.Timestamp, assignment.Month
            );
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating assignment");
            return Error.InternalServerError("Error al crear el reconocimiento");
        }
    }
}
