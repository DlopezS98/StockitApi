using Microsoft.EntityFrameworkCore;
using StockitApi.Data;
using StockitApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddPermissionAuthorization();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("ApplyMigrations"))
{
    using IServiceScope scope = app.Services.CreateScope();
    StockitContext dbContext = scope.ServiceProvider.GetRequiredService<StockitContext>();
    await dbContext.Database.MigrateAsync();
}

using (IServiceScope scope = app.Services.CreateScope())
{
    await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
