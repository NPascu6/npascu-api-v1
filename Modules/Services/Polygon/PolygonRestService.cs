using System.Collections.Concurrent;
using System.Net;
using Microsoft.AspNetCore.SignalR;
using npascu_api_v1.Modules.DTOs;
using npascu_api_v1.Modules.Quote;

namespace npascu_api_v1.Modules.Services.Polygon;

public class PolygonRestService : BackgroundService
{
    private readonly ILogger<PolygonRestService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly IHubContext<QuotesHub> _hubContext;
    private readonly List<string> _symbols;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromMinutes(1);

    public static ConcurrentDictionary<string, PolygonQuoteDto> LatestQuotes { get; } = new();

    public PolygonRestService(IConfiguration configuration, ILogger<PolygonRestService> logger,
        HttpClient httpClient, IHubContext<QuotesHub> hubContext)
    {
        _logger = logger;
        _httpClient = httpClient;
        _hubContext = hubContext;
        _apiKey = configuration["POLYGON_API_KEY"] ?? "demo";
        _symbols = configuration["POLYGON_SYMBOLS"]?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList() ?? new List<string> { "AAPL", "MSFT", "GOOGL" };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var symbol in _symbols)
            {
                await PollAsync(symbol, stoppingToken);
            }

            try
            {
                await Task.Delay(_pollingInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task PollAsync(string symbol, CancellationToken cancellationToken)
    {
        var url = $"https://api.polygon.io/v1/last/stocks/{symbol}?apiKey={_apiKey}";
        try
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var quoteResponse = await response.Content.ReadFromJsonAsync<PolygonQuoteResponse>(cancellationToken: cancellationToken);
                if (quoteResponse?.Last != null)
                {
                    var quote = quoteResponse.Last;
                    LatestQuotes[symbol] = quote;
                    await _hubContext.Clients.All.SendAsync("ReceiveQuote", symbol, quote, cancellationToken);
                }
            }
            else if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _logger.LogWarning("Polygon rate limit hit for {Symbol}", symbol);
            }
            else
            {
                _logger.LogWarning("Polygon error {Status} for {Symbol}", response.StatusCode, symbol);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error polling Polygon for {Symbol}", symbol);
        }
    }
}
