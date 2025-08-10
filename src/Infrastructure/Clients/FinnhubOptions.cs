namespace Infrastructure.Clients;

/// <summary>
/// Configuration for Finnhub service.
/// </summary>
public class FinnhubOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string RestBaseUrl { get; set; } = "https://finnhub.io/api/v1/";
    public string WebSocketUrl { get; set; } = "wss://ws.finnhub.io";
}
