using System.Diagnostics;
using StockitApi.Entities;
using StockitApi.Interfaces;

namespace StockitApi.Middlewares;

// Records one ApiUsageLog row per request (endpoint, user, status, latency) - powers the
// "usage metrics" screen in the mobile admin app.
public class UsageMetricsMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork, IAppRequestContext appRequestContext)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        await _next(context);
        stopwatch.Stop();

        try
        {
            int? userId = context.User.Identity?.IsAuthenticated == true
                ? appRequestContext.GetCurrentUserId()
                : null;

            await unitOfWork.ApiUsageLogs.AddAsync(new ApiUsageLog
            {
                UserId = userId,
                Endpoint = context.Request.Path,
                HttpMethod = context.Request.Method,
                StatusCode = context.Response.StatusCode,
                IpAddress = appRequestContext.IpAddress,
                UserAgent = appRequestContext.UserAgent,
                ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds,
            });
            await unitOfWork.SaveChangesAsync();
        }
        catch
        {
            // Never let logging failures break the response.
        }
    }
}

