using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Dtos.Assignments;
using Services.Interfaces;
using Shared.ResultPattern;
using System.Security.Claims;
using WebApi.Utils;

namespace WebApi.Controllers;

[ApiController]
[Route("assignments")]
[Authorize]
public class AssignmentsController(IAssignmentsService assignmentsService, IValidator<CreateAssignmentDto> validator) : ControllerBase
{
    private readonly IAssignmentsService _assignmentsService = assignmentsService;
    private readonly IValidator<CreateAssignmentDto> _validator = validator;

    [HttpGet("received")]
    public async Task<ActionResult<Result<List<AssignmentDto>>>> GetReceived()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var result = await _assignmentsService.GetReceivedAssignmentsAsync(userId);
        return result.GetResponse();
    }

    [HttpGet]
    [Authorize(Roles = "people")]
    public async Task<ActionResult<Result<List<AssignmentDto>>>> GetAll()
    {
        var result = await _assignmentsService.GetAllAssignmentsAsync();
        return result.GetResponse();
    }

    [HttpPost]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Result<AssignmentDto>>> Create([FromBody] CreateAssignmentDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.MapToTypedResult<AssignmentDto>());

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var result = await _assignmentsService.CreateAssignmentAsync(userId, dto);
        return result.GetResponse();
    }
}
