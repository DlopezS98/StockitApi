using System.ComponentModel.DataAnnotations;

namespace StockitApi.Dtos;

public record AuthRequest(
    [Required, EmailAddress] string Email, 
    [Required, MinLength(8)] string Password
);

public record RegisterRequest(
    [Required, EmailAddress] string Email, 
    [Required, MinLength(8)] string Password, 
    [Required] string FirstName, 
    [Required] string LastName,
    [Required] int RoleId
);