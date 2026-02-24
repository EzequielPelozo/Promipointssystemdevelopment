namespace Services.Dtos.Assignments;

public record AssignmentDto(
    int Id,
    int FromUserId,
    string FromUserName,
    int ToUserId,
    string ToUserName,
    int Points,
    string Category,
    string? Message,
    long Timestamp,
    string Month
);
