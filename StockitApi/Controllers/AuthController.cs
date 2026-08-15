using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockitApi.Dtos;
using StockitApi.Interfaces;
using StockitApi.Entities;

namespace StockitApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class AuthController(IAuthenticationService authenticationService, IUnitOfWork unitOfWork, IAppRequestContext appRequestContext) : ControllerBase
{
    private readonly IAuthenticationService _authService = authenticationService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
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
    [HttpPost("register")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Created(string.Empty, result);
    }

    // [Authorize]
    // [HttpGet("me")]
    // [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    // // [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(HttpErrorResponse))]
    // public async Task<IActionResult> GetCurrentUser()
    // {
    //     User? result = await _unitOfWork.Users.GetByIdAsync(_appRequestContext.GetCurrentUserId());
    //     if (result is null) return NotFound();

    //     var userResponse = new UserResponse(
    //         result.Username, 
    //         result.Email, 
    //         result.FullName, 
    //         result.RoleId, 
    //         result.Role?.Name,
    //         result.CreatedAt
    //     );

    //     return Ok(result);
    // }
}
