using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using Domain.DTOs;
using Api.Hubs;

namespace Api.Background;

public class FinnhubRestService : BackgroundService
{
    private readonly ILogger<FinnhubRestService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly IHubContext<QuotesHub> _hubContext;
    private const string BaseUrl = "https://finnhub.io/api/v1/quote";
    private const string TradesUrl = "https://finnhub.io/api/v1/stock/trades";
    private const int MaxRequestsPerMinute = 60;

    private readonly TimeSpan _requestInterval;
    private readonly List<string> _symbols;

    public static ConcurrentDictionary<string, FinnhubQuoteDto> LatestQuotes { get; } = new();
    public static ConcurrentDictionary<string, FinnhubTradeTick> LatestTrades { get; } = new();

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

        _requestInterval = TimeSpan.FromMinutes(1) / MaxRequestsPerMinute;
        _logger.LogInformation(
            "Request interval set to {Seconds}s for {Count} symbols (approx {SymbolInterval}s per symbol).",
            _requestInterval.TotalSeconds,
            _symbols.Count,
            _requestInterval.TotalSeconds * Math.Max(1, _symbols.Count));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting sequential polling respecting rate limits.");
        foreach (var symbol in _symbols)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            await PollFinnhubAsync(symbol, stoppingToken);

            try
            {
                await Task.Delay(_requestInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task PollFinnhubAsync(string symbol, CancellationToken token)
    {
        await FetchQuoteAsync(symbol);
        try
        {
            await Task.Delay(_requestInterval, token);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        await FetchLatestTradeAsync(symbol);
    }

    private async Task FetchQuoteAsync(string symbol)
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
                _logger.LogWarning("Rate limit hit when fetching data for symbol {Symbol}. Backing off for {Delay}s.", symbol, _requestInterval.TotalSeconds);
                await Task.Delay(_requestInterval);
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

    private async Task FetchLatestTradeAsync(string symbol)
    {
        var now = DateTime.UtcNow;
        var from = now.AddMinutes(-5);
        var url = $"{TradesUrl}?symbol={symbol}&from={from:yyyy-MM-dd}&to={now:yyyy-MM-dd}&token={_apiKey}&limit=1";
        try
        {
            using var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                if (response.Content.Headers.ContentType?.MediaType != "application/json")
                {
                    var body = await response.Content.ReadAsStringAsync();
                    body = Regex.Replace(body, "<script[\\s\\S]*?</script>", string.Empty, RegexOptions.IgnoreCase);
                    if (body.Length > 200)
                        body = body[..200] + "...";
                    _logger.LogWarning(
                        "Unexpected content type when fetching trades for {Symbol}: {MediaType}. Body: {Body}",
                        symbol,
                        response.Content.Headers.ContentType?.MediaType,
                        body);
                    return;
                }

                try
                {
                    var tradeDto = await response.Content.ReadFromJsonAsync<FinnhubTradeDto>();
                    var trade = tradeDto?.data?.FirstOrDefault();
                    if (trade != null)
                    {
                        if (LatestTrades.TryGetValue(symbol, out var prev) && prev.t >= trade.t)
                            return;

                        LatestTrades[symbol] = trade;
                        await _hubContext.Clients.All.SendAsync("ReceiveTrade", symbol, trade);
                    }
                }
                catch (JsonException ex)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogError(ex, "Failed to parse trade data for {Symbol}. Body: {Body}", symbol, body);
                }
            }
            else if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _logger.LogWarning("Rate limit hit when fetching trades for {Symbol}.", symbol);
                await Task.Delay(_requestInterval);
            }
            else
            {
                _logger.LogError("Error fetching trades for symbol {Symbol}. HTTP status: {StatusCode}", symbol, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while fetching trades for symbol {Symbol}.", symbol);
        }
    }
}
