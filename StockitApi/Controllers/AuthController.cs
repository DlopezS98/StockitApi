using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockitApi.Authorization;
using StockitApi.Dtos;
using StockitApi.Interfaces;
using StockitApi.Models;

namespace StockitApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class AuthController(IAuthenticationService authenticationService, IUserService userService, IAppRequestContext appRequestContext) : ControllerBase
{
    private readonly IAuthenticationService _authService = authenticationService;
    private readonly IUserService _userService = userService;
    private readonly IAppRequestContext _appRequestContext = appRequestContext;

    [HttpPost("login")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var result = await _authService.AuthenticateAsync(request);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        await _authService.LogoutAsync(request.RefreshToken);
        return NoContent();
    }

    [RequirePermission(Permission.User.Write)]
    [HttpPost("register")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Created(string.Empty, result);
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser()
    {
        UserResponse? result = await _userService.GetByIdAsync(_appRequestContext.GetCurrentUserId());
        if (result is null) return NotFound();

        return Ok(result);
    }

    [Authorize]
    [HttpGet("sessions")]
    [ProducesResponseType<IReadOnlyList<SessionResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveSessions()
    {
        IReadOnlyList<SessionResponse> result = await _authService.GetActiveSessionsAsync(_appRequestContext.GetCurrentUserId());
        return Ok(result);
    }

    [Authorize]
    [HttpPost("sessions/{id:guid}/revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeSession(Guid id)
    {
        await _authService.RevokeSessionAsync(_appRequestContext.GetCurrentUserId(), id);
        return NoContent();
    }

    [Authorize]
    [HttpPost("sessions/revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeAllSessions([FromBody] RevokeAllSessionsRequest? request, [FromQuery] bool keepCurrent = false)
    {
        if (keepCurrent && string.IsNullOrEmpty(request?.RefreshToken))
            return BadRequest("RefreshToken must be provided to keep the current session.");

        string? currentRefreshToken = keepCurrent ? request!.RefreshToken : null;
        await _authService.RevokeAllSessionsAsync(_appRequestContext.GetCurrentUserId(), currentRefreshToken);
        return NoContent();
    }
}

