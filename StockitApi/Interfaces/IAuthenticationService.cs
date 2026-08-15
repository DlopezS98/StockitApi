using StockitApi.Dtos;

namespace StockitApi.Interfaces;

public interface IAuthenticationService
{
    Task<TokenResponse> AuthenticateAsync(AuthRequest request);
    Task<UserResponse> RegisterAsync(RegisterRequest request);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
}