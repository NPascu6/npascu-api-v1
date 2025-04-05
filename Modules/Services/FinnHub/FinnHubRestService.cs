using System.Collections.Concurrent;
using System.Net;
using Microsoft.AspNetCore.SignalR;
using npascu_api_v1.Modules.DTOs;
using npascu_api_v1.Modules.Quote;

namespace npascu_api_v1.Modules.Services.FinnHub;

public class FinnHubRestService : BackgroundService
{
    private readonly ILogger<FinnHubRestService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly IHubContext<QuotesHub> _hubContext;
    private static readonly string BaseUrl = "https://finnhub.io/api/v1/quote";
    private readonly List<string> _symbols;

    public static ConcurrentDictionary<string, FinnhubQuoteDto> LatestQuotes { get; } = new();

    public FinnHubRestService(
        IConfiguration configuration,
        ILogger<FinnHubRestService> logger,
        HttpClient httpClient,
        IHubContext<QuotesHub> hubContext)
    {
        _logger = logger;
        _httpClient = httpClient;
        _hubContext = hubContext;

        _apiKey = configuration["FINNHUB_API_KEY"]
                  ?? throw new InvalidOperationException("Finnhub API key not configured.");

        _symbols = configuration["FINNHUB_SYMBOLS"]?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList() ?? ["AAPL", "MSFT", "GOOGL"];

        if (_symbols.Count == 0)
        {
            _logger.LogWarning("No symbols configured for polling. Please check the FINNHUB_SYMBOLS configuration.");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_symbols.Any())
        {
            _logger.LogWarning("No symbols to poll. Service will not start.");
            return;
        }

        _logger.LogInformation("Starting sequential round-robin polling for symbols.");

        var symbolQueue = new Queue<string>(_symbols);

        await PollSymbolsSequentially(symbolQueue, stoppingToken);
    }

    private async Task PollSymbolsSequentially(Queue<string> symbolQueue, CancellationToken cancellationToken)
    {
        try
        {
            if (symbolQueue.Count == 0)
            {
                foreach (var symbol in _symbols)
                {
                    symbolQueue.Enqueue(symbol);
                }
            }

            var nextSymbol = symbolQueue.Dequeue();
            await PollFinnhubAsync(nextSymbol, cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

            // Recursively continue
            await PollSymbolsSequentially(symbolQueue, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("FinnHub REST Service polling canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during polling cycle.");
        }
    }

    private async Task PollFinnhubAsync(string symbol, CancellationToken cancellationToken)
    {
        var url = $"{BaseUrl}?symbol={symbol}&token={_apiKey}";

        try
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var quote =
                    await response.Content.ReadFromJsonAsync<FinnhubQuoteDto>(cancellationToken: cancellationToken);
                if (quote != null)
                {
                    LatestQuotes[symbol] = quote;

                    _logger.LogInformation(
                        "Symbol: {Symbol}, Price: {Price}, High: {High}, Low: {Low}, Open: {Open}, PrevClose: {PrevClose}, Timestamp: {Timestamp}",
                        symbol, quote.c, quote.h, quote.l, quote.o, quote.pc, quote.t);

                    await _hubContext.Clients.All.SendAsync("ReceiveQuote", symbol, quote, cancellationToken);
                }
                else
                {
                    _logger.LogError("Received empty quote data for symbol {Symbol}.", symbol);
                }
            }
            else if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _logger.LogWarning("Rate limit hit while fetching data for symbol {Symbol}.", symbol);
            }
            else
            {
                _logger.LogError("Error fetching data for symbol {Symbol}. HTTP status: {StatusCode}",
                    symbol, response.StatusCode);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Polling canceled for symbol {Symbol}.", symbol);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while fetching data for symbol {Symbol}.", symbol);
        }
    }
}