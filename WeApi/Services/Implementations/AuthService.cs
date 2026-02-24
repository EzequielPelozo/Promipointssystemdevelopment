using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Services.Dtos.Auth;
using Services.Dtos.Users;
using Services.Interfaces;
using Services.Mappers;
using Shared.ResultPattern;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.Implementations;

public class AuthService(IUnitOfWork unitOfWork, IConfiguration configuration) : IAuthService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IConfiguration _configuration = configuration;

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _unitOfWork.Users.GetByEmail(dto.Email);
        if (user is null)
        {
            Log.Warning("Login failed: user {Email} not found", dto.Email);
            return Error.Unauthorized("Credenciales inválidas");
        }

        bool validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!validPassword)
        {
            Log.Warning("Login failed: invalid password for {Email}", dto.Email);
            return Error.Unauthorized("Credenciales inválidas");
        }

        var token = GenerateJwtToken(user.Id, user.Email, user.Role);
        var userDto = user.ToDto();

        Log.Information("User {Email} logged in successfully", dto.Email);
        return new LoginResponseDto(token, userDto);
    }

    public async Task<Result<UserDto>> GetMeAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetById(userId);
        if (user is null)
            return Error.RecordNotFound("Usuario no encontrado");

        return user.ToDto();
    }

    private string GenerateJwtToken(int userId, string email, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
        };

        var expHours = int.Parse(_configuration["Jwt:ExpirationHours"] ?? "8");
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
