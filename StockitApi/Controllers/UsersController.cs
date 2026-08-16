using System.Net.Mime;
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
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [RequirePermission(Permission.User.Read)]
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<UserResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        IReadOnlyList<UserResponse> result = await _userService.GetAllAsync();
        return Ok(result);
    }

    [RequirePermission(Permission.User.Read)]
    [HttpGet("{id:int}")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        UserResponse? result = await _userService.GetByIdAsync(id);
        if (result is null) return NotFound();

        return Ok(result);
    }

    [RequirePermission(Permission.User.Write)]
    [HttpPut("{id:int}")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        UserResponse result = await _userService.UpdateAsync(id, request);
        return Ok(result);
    }

    [RequirePermission(Permission.User.Delete)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Deactivate(int id)
    {
        await _userService.DeactivateAsync(id);
        return NoContent();
    }

    [RequirePermission(Permission.User.Write)]
    [HttpPost("{id:int}/change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
    {
        await _userService.ChangePasswordAsync(id, request);
        return NoContent();
    }
}
