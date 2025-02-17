using npascu_api_v1.Modules.DTOs;
using System.Net;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using npascu_api_v1.Modules.Hub;

namespace npascu_api_v1.Modules.Background
{
    public class FinnhubRestService : BackgroundService
    {
        private readonly ILogger<FinnhubRestService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IHubContext<QuotesHub> _hubContext;
        private const string BaseUrl = "https://finnhub.io/api/v1/quote";

        // With 10 symbols and a limit of 60 requests/minute,
        // poll each symbol every 10 seconds.
        private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(10);

        // List of symbols to poll.
        private readonly List<string> _symbols;

        // Initialize the cache of the latest quotes.
        public static ConcurrentDictionary<string, FinnhubQuoteDto> LatestQuotes { get; } =
            new ConcurrentDictionary<string, FinnhubQuoteDto>();

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
                _logger.LogWarning(
                    "No symbols configured for polling. Please check the FINNHUB_SYMBOLS configuration.");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting concurrent polling for symbols via timers.");

            // Create a dedicated polling loop for each symbol.
            var pollingTasks = _symbols.Select(symbol => PollSymbolLoop(symbol, stoppingToken));
            await Task.WhenAll(pollingTasks);
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
                        // Update the cache.
                        LatestQuotes[symbol] = quote;

                        // Log the full quote.
                        _logger.LogInformation(
                            "Symbol: {Symbol}, Price: {Price}, High: {High}, Low: {Low}, Open: {Open}, PrevClose: {PrevClose}, Timestamp: {Timestamp}",
                            symbol, quote.c, quote.h, quote.l, quote.o, quote.pc, quote.t);

                        // Push the update via SignalR.
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
                    _logger.LogError("Error fetching data for symbol {Symbol}. HTTP status: {StatusCode}",
                        symbol, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while fetching data for symbol {Symbol}.", symbol);
            }
        }
    }
}