using System.Text.Json;
using System.Text.Json.Serialization;

namespace npascu_api_v1.Modules.Services.AlphaVantage;

public class AlphaVantageHistoricalService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AlphaVantageHistoricalService> _logger;
    private readonly string _apiKey;
    private const string BaseUrl = "https://www.alphavantage.co/query";

    public AlphaVantageHistoricalService(HttpClient httpClient, IConfiguration configuration, ILogger<AlphaVantageHistoricalService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["ALPHA_VANTAGE_API_KEY"] ?? throw new InvalidOperationException("AlphaVantage API key not configured.");
    }

    public async Task<Dictionary<string, AlphaVantageDailyBarDto>?> GetDailyHistoryAsync(string symbol, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={_apiKey}";
        try
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var raw = await response.Content.ReadAsStringAsync(cancellationToken);
                var parsed = JsonSerializer.Deserialize<AlphaVantageDailyResponse>(raw);
                return parsed?.TimeSeriesDaily;
            }

            _logger.LogWarning("Failed to fetch historical data for {Symbol}. Status {Status}", symbol, response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching historical data for {Symbol}", symbol);
        }

        return null;
    }
}

public class AlphaVantageDailyResponse
{
    [JsonPropertyName("Time Series (Daily)")]
    public Dictionary<string, AlphaVantageDailyBarDto>? TimeSeriesDaily { get; set; }
}

public class AlphaVantageDailyBarDto
{
    [JsonPropertyName("1. open")]
    public decimal Open { get; set; }

    [JsonPropertyName("2. high")]
    public decimal High { get; set; }

    [JsonPropertyName("3. low")]
    public decimal Low { get; set; }

    [JsonPropertyName("4. close")]
    public decimal Close { get; set; }

    [JsonPropertyName("5. volume")]
    public long Volume { get; set; }
}
