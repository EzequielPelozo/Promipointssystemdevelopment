using Services.Dtos.Auth;
using Services.Dtos.Users;
using Shared.ResultPattern;

namespace Services.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(LoginDto dto);
    Task<Result<UserDto>> GetMeAsync(int userId);
}
