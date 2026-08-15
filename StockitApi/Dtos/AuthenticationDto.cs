namespace StockitApi.Dtos;

public record AuthRequest(string Email, string Password);
public record RegisterRequest(
    string Email, 
    string Password, 
    string FirstName, 
    string LastName,
    int RoleId
);