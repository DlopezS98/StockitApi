namespace StockitApi.Interfaces;

public interface IAppRequestContext
{
    /// <summary>
    /// Gets the current user's ID from the request context. If not present or invalid, throws an InvalidOperationException.
    /// </summary>
    /// <returns></returns>
    int GetCurrentUserId();
    string IpAddress { get; }
    string UserAgent { get; }
    string DeviceName { get; }
}