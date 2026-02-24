using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Dtos.Auth;
using Services.Dtos.Users;
using Services.Interfaces;
using Shared.ResultPattern;
using System.Security.Claims;
using WebApi.Utils;

namespace WebApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService, IValidator<LoginDto> validator) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IValidator<LoginDto> _validator = validator;

    [HttpPost("login")]
    public async Task<ActionResult<Result<LoginResponseDto>>> Login([FromBody] LoginDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.MapToTypedResult<LoginResponseDto>());

        var result = await _authService.LoginAsync(dto);
        return result.GetResponse();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<Result<UserDto>>> GetMe()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var result = await _authService.GetMeAsync(userId);
        return result.GetResponse();
    }
}
