using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Dtos.Users;
using Services.Interfaces;
using Shared.ResultPattern;
using WebApi.Utils;

namespace WebApi.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public class UsersController(IUsersService usersService) : ControllerBase
{
    private readonly IUsersService _usersService = usersService;

    [HttpGet]
    public async Task<ActionResult<Result<List<UserDto>>>> GetAll()
    {
        var result = await _usersService.GetAllUsersAsync();
        return result.GetResponse();
    }
}
