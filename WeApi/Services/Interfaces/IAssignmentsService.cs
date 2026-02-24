using Services.Dtos.Assignments;
using Shared.ResultPattern;

namespace Services.Interfaces;

public interface IAssignmentsService
{
    Task<Result<List<AssignmentDto>>> GetReceivedAssignmentsAsync(int userId);
    Task<Result<List<AssignmentDto>>> GetAllAssignmentsAsync();
    Task<Result<AssignmentDto>> CreateAssignmentAsync(int fromUserId, CreateAssignmentDto dto);
}
