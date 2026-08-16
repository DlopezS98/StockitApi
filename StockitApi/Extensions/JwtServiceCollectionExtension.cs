using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StockitApi.Authorization;
using StockitApi.Entities;
using StockitApi.Interfaces;
using StockitApi.Models;

namespace StockitApi.Extensions;

public static class JwtServiceCollectionExtension
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        JwtOptions jwtOptions = configuration.GetRequiredSection(JwtOptions.SectionName).Get<JwtOptions>()!;
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(jwtOptions.SigningKey));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true
            };
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    string? refreshTokenIdClaim = context.Principal?.FindFirstValue(PermissionAuthorization.RefreshTokenIdClaimType);
                    if (refreshTokenIdClaim is null || !Guid.TryParse(refreshTokenIdClaim, out Guid refreshTokenId))
                    {
                        context.Fail("Invalid token.");
                        return;
                    }

                    IUnitOfWork unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                    RefreshToken? refreshToken = await unitOfWork.RefreshTokens.GetByIdAsync(refreshTokenId);
                    if (refreshToken is null || refreshToken.IsRevoked)
                        context.Fail("Session has been revoked.");
                }
            };
        });

        return services;
    }
}