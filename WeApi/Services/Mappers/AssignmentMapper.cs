using Business.Entities;
using Services.Dtos.Assignments;

namespace Services.Mappers;

public static class AssignmentMapper
{
    public static AssignmentDto ToDto(this PointAssignment assignment) =>
        new(
            assignment.Id,
            assignment.FromUserId,
            assignment.FromUser?.Name ?? string.Empty,
            assignment.ToUserId,
            assignment.ToUser?.Name ?? string.Empty,
            assignment.Points,
            assignment.Category,
            assignment.Message,
            assignment.Timestamp,
            assignment.Month
        );
}
