using StockitApi.Interfaces;
using StockitApi.Services;

namespace StockitApi.Extensions;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<IObservabilityService, ObservabilityService>();

        return services;
    }
}