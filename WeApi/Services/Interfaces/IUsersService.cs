using Services.Dtos.Users;
using Shared.ResultPattern;

namespace Services.Interfaces;

public interface IUsersService
{
    Task<Result<List<UserDto>>> GetAllUsersAsync();
}
