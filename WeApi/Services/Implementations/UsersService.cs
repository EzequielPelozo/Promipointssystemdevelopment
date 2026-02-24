using DataAccess.Interfaces;
using Serilog;
using Services.Dtos.Users;
using Services.Interfaces;
using Services.Mappers;
using Shared.ResultPattern;

namespace Services.Implementations;

public class UsersService(IUnitOfWork unitOfWork) : IUsersService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<List<UserDto>>> GetAllUsersAsync()
    {
        try
        {
            var users = await _unitOfWork.Users.GetAll(isTracking: false);
            return users.Select(u => u.ToDto()).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching users");
            return Error.InternalServerError("Error al obtener usuarios");
        }
    }
}
