using StockitApi.Dtos;
using StockitApi.Entities;

namespace StockitApi.Interfaces;

public interface ITokenService
{
    (string token, DateTime expiresAtUtc) GenerateToken(User user, Guid refreshTokenId);
    RefreshTokenResult GenerateRefreshToken();
    string ComputeHash(string raw);
}
