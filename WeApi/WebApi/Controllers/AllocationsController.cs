using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Dtos.Allocations;
using Services.Interfaces;
using Shared.ResultPattern;
using System.Security.Claims;
using WebApi.Utils;

namespace WebApi.Controllers;

[ApiController]
[Route("allocations")]
[Authorize]
public class AllocationsController(IAllocationsService allocationsService) : ControllerBase
{
    private readonly IAllocationsService _allocationsService = allocationsService;

    [HttpGet("me/{month}")]
    public async Task<ActionResult<Result<AllocationDto>>> GetMyAllocation(string month)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var result = await _allocationsService.GetMyAllocationAsync(userId, month);
        return result.GetResponse();
    }
}
