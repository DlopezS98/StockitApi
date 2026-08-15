using StockitApi.Dtos;
using StockitApi.Entities;

namespace StockitApi.Interfaces;

public interface ITokenService
{
    (string token, DateTime expiresAtUtc) GenerateToken(User user);
    RefreshTokenResult GenerateRefreshToken();
    string ComputeHash(string raw);
}
