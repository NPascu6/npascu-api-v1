using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR;
using Api.DTOs;
using Api.Hubs;

namespace Api.Background;

public class FinnhubRestService : BackgroundService
{
    private readonly ILogger<FinnhubRestService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly IHubContext<QuotesHub> _hubContext;
    private const string BaseUrl = "https://finnhub.io/api/v1/quote";
    private const int MaxRequestsPerMinute = 60;

    private readonly TimeSpan _pollingInterval;
    private readonly List<string> _symbols;

    public static ConcurrentDictionary<string, FinnhubQuoteDto> LatestQuotes { get; } = new();

    public FinnhubRestService(IConfiguration configuration, ILogger<FinnhubRestService> logger,
        HttpClient httpClient, IHubContext<QuotesHub> hubContext)
    {
        _logger = logger;
        _httpClient = httpClient;
        _hubContext = hubContext;
        _apiKey = configuration["FINNHUB_API_KEY"] ?? throw new Exception("Finhub API key not configured.");

        var symbolsConfig = configuration["FINNHUB_SYMBOLS"];
        if (!string.IsNullOrWhiteSpace(symbolsConfig))
        {
            _symbols = symbolsConfig.Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }
        else
        {
            _symbols = new List<string> { "AAPL", "MSFT", "GOOGL" };
        }

        if (_symbols.Count == 0)
        {
            _logger.LogWarning("No symbols configured for polling. Please check the FINNHUB_SYMBOLS configuration.");
        }

        var requestsPerMinutePerSymbol = (double)MaxRequestsPerMinute / Math.Max(1, _symbols.Count);
        var secondsPerRequest = Math.Max(1, 60d / requestsPerMinutePerSymbol);
        _pollingInterval = TimeSpan.FromSeconds(secondsPerRequest);
        _logger.LogInformation("Polling interval set to {Seconds}s for {Count} symbols.", _pollingInterval.TotalSeconds, _symbols.Count);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting concurrent polling for symbols via timers.");
        var tasks = _symbols.Select(symbol => PollSymbolLoop(symbol, stoppingToken));
        await Task.WhenAll(tasks);
    }

    private async Task PollSymbolLoop(string symbol, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting polling loop for symbol {Symbol}.", symbol);
        while (!cancellationToken.IsCancellationRequested)
        {
            await PollFinnhubAsync(symbol);
            try
            {
                await Task.Delay(_pollingInterval, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
        _logger.LogInformation("Polling loop stopped for symbol {Symbol}.", symbol);
    }

    private async Task PollFinnhubAsync(string symbol)
    {
        var url = $"{BaseUrl}?symbol={symbol}&token={_apiKey}";
        try
        {
            using var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var quote = await response.Content.ReadFromJsonAsync<FinnhubQuoteDto>();
                if (quote != null)
                {
                    LatestQuotes[symbol] = quote;
                    _logger.LogInformation("Symbol: {Symbol}, Price: {Price}", symbol, quote.c);
                    await _hubContext.Clients.All.SendAsync("ReceiveQuote", symbol, quote);
                }
                else
                {
                    _logger.LogError("Received empty quote data for symbol {Symbol}.", symbol);
                }
            }
            else if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _logger.LogError("Rate limit hit when fetching data for symbol {Symbol}.", symbol);
            }
            else
            {
                _logger.LogError("Error fetching data for symbol {Symbol}. HTTP status: {StatusCode}", symbol, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while fetching data for symbol {Symbol}.", symbol);
        }
    }
}
