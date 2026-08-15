namespace StockitApi.Interfaces;

public interface IAppRequestContext
{
    string UserId { get; }
    string IpAddress { get; }
    string UserAgent { get; }
    string DeviceName { get; }
}