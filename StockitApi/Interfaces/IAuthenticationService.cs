using StockitApi.Dtos;

namespace StockitApi.Interfaces;

public interface IAuthenticationService
{
    Task<TokenResponse> AuthenticateAsync(AuthRequest request);
    Task<UserResponse> RegisterAsync(AuthRequest request);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
}