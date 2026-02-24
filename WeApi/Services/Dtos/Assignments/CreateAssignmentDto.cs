namespace Services.Dtos.Assignments;

public record CreateAssignmentDto(int ToUserId, int Points, string Category, string? Message);
