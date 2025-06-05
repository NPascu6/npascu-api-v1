using System.Collections.Concurrent;
using System.Net;
using Microsoft.AspNetCore.SignalR;
using npascu_api_v1.Modules.DTOs;
using npascu_api_v1.Modules.Quote;

namespace npascu_api_v1.Modules.Services.YahooFinance;

public class YahooFinanceRestService : BackgroundService
{
    private readonly ILogger<YahooFinanceRestService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IHubContext<QuotesHub> _hubContext;
    private readonly List<string> _symbols;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromMinutes(1);

    public static ConcurrentDictionary<string, YahooQuoteDto> LatestQuotes { get; } = new();

    public YahooFinanceRestService(IConfiguration configuration, ILogger<YahooFinanceRestService> logger,
        HttpClient httpClient, IHubContext<QuotesHub> hubContext)
    {
        _logger = logger;
        _httpClient = httpClient;
        _hubContext = hubContext;
        _symbols = configuration["YAHOO_SYMBOLS"]?
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
        var url = $"https://query1.finance.yahoo.com/v7/finance/quote?symbols={symbol}";
        try
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var quoteResponse = await response.Content.ReadFromJsonAsync<YahooQuoteResponse>(cancellationToken: cancellationToken);
                var quote = quoteResponse?.QuoteResponse?.Result?.FirstOrDefault();
                if (quote != null)
                {
                    LatestQuotes[symbol] = quote;
                    await _hubContext.Clients.All.SendAsync("ReceiveQuote", symbol, quote, cancellationToken);
                }
            }
            else if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _logger.LogWarning("Yahoo Finance rate limit hit for {Symbol}", symbol);
            }
            else
            {
                _logger.LogWarning("Yahoo Finance error {Status} for {Symbol}", response.StatusCode, symbol);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error polling Yahoo Finance for {Symbol}", symbol);
        }
    }
}
