using Services.Dtos.Users;

namespace Services.Dtos.Auth;

public record LoginResponseDto(string Token, UserDto User);
