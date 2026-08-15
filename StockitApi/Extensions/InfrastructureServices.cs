using StockitApi.Data;
using Microsoft.EntityFrameworkCore;
using StockitApi.Interfaces;
using StockitApi.Data.Repositories;
using StockitApi.Models;
using StockitApi.Services;

namespace StockitApi.Extensions;

public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        services.AddDbContext<StockitContext>(options => options.UseSqlServer(connectionString));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAppRequestContext, AppRequestContext>();

        return services;
    }
}