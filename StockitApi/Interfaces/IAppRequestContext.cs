namespace StockitApi.Interfaces;

public interface IAppRequestContext
{
    int GetCurrentUserId();
    string IpAddress { get; }
    string UserAgent { get; }
    string DeviceName { get; }
}