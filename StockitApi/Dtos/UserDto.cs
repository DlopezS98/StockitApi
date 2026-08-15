using System.ComponentModel.DataAnnotations;

namespace StockitApi.Dtos;

public record UserResponse(
    int Id,
    string Username,
    string Email,
    string FullName,
    int RoleId,
    string RoleName,
    DateTime CreatedAt
);

public record UpdateUserRequest(
    [Required] string FirstName,
    [Required] string LastName,
    [Required] int RoleId
);

public record ChangePasswordRequest(
    [Required] string CurrentPassword,
    [Required, MinLength(8)] string NewPassword
);