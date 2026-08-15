using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StockitApi.Authorization;
using StockitApi.Dtos;
using StockitApi.Entities;
using StockitApi.Interfaces;
using StockitApi.Models;

namespace StockitApi.Services;

public class TokenService(IOptions<JwtOptions> jwtOptions, TimeProvider timeProvider) : ITokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly TimeProvider _timeProvider = timeProvider;

    public string ComputeHash(string raw)
    {
        var hashed = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToBase64String(hashed);
    }

    public RefreshTokenResult GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var rawToken = Convert.ToBase64String(bytes);
        var hashedToken = ComputeHash(rawToken);
        DateTimeOffset expiresAt = _timeProvider.GetUtcNow().AddDays(_jwtOptions.RefreshTokenDays);
        return new RefreshTokenResult(rawToken, hashedToken, expiresAt.UtcDateTime);
    }

    public (string token, DateTime expiresAtUtc) GenerateToken(User user)
    {
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        DateTime expiresAtUtc = _timeProvider.GetUtcNow()
            .AddMinutes(_jwtOptions.AccessTokenMinutes)
            .UtcDateTime;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
        };

        IEnumerable<string> permissionCodes = user.Role!.RolePermissions
            .Select(rp => rp.Permission!.Code)
            .Where(code => code is not null)! ?? [];
        claims.AddRange(permissionCodes.Select(code => new Claim(PermissionAuthorization.PermissionClaimType, code)));

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials
        );
        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}