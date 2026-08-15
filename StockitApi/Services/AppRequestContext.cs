using System.Security.Claims;
using StockitApi.Interfaces;

namespace StockitApi.Services;

public class AppRequestContext : IAppRequestContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppRequestContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    public string IpAddress => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty;
    public string UserAgent => _httpContextAccessor.HttpContext?.Request?.Headers.UserAgent.ToString() ?? string.Empty;
    public string DeviceName => _httpContextAccessor.HttpContext?.Request?.Headers["Device-Name"].ToString() ?? string.Empty;
}