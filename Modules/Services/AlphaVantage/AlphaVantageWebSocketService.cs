using System.Collections.Concurrent;
using System.Net;
using Microsoft.AspNetCore.SignalR;
using npascu_api_v1.Modules.DTOs;
using npascu_api_v1.Modules.Quote;

namespace npascu_api_v1.Modules.Services.AlphaVantage
{
    public class AlphaVantageRestService : BackgroundService
    {
        private readonly ILogger<AlphaVantageRestService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IHubContext<QuotesHub> _hubContext;
        private const string BaseUrl = "https://www.alphavantage.co/query";

        // Alpha Vantage free tier allows only 25 requests per day.
        // This calculates a safe polling interval based on the number of symbols.
        private readonly TimeSpan _pollingInterval;

        // List of symbols to poll.
        private readonly List<string> _symbols;

        // Cache of the latest quotes.
        public static ConcurrentDictionary<string, GlobalQuoteDto> LatestQuotes { get; } =
            new ConcurrentDictionary<string, GlobalQuoteDto>();

        public AlphaVantageRestService(IConfiguration configuration, ILogger<AlphaVantageRestService> logger,
            HttpClient httpClient, IHubContext<QuotesHub> hubContext)
        {
            _logger = logger;
            _httpClient = httpClient;
            _hubContext = hubContext;
            _apiKey = configuration["ALPHA_VANTAGE_API_KEY"] ??
                      throw new Exception("AlphaVantage API key not configured.");

            var symbolsConfig = configuration["ALPHA_VANTAGE_SYMBOLS"];
            if (!string.IsNullOrWhiteSpace(symbolsConfig))
            {
                _symbols = symbolsConfig.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();
            }
            else
            {
                _symbols = ["IBM", "AAPL", "MSFT", "NVDA", "TSLA"];
            }

            if (_symbols.Count == 0)
            {
                _logger.LogWarning(
                    "No symbols configured for polling. Please check the ALPHA_VANTAGE_SYMBOLS configuration.");
            }

            // Calculate polling interval based on allowed requests:
            // Total seconds in a day divided by (allowed requests per day / number of symbols).
            // For 25 requests/day and N symbols, each symbol can be polled roughly:
            // Interval = (24 * 3600 * N) / 25 seconds.
            int allowedRequestsPerDay = 25;
            int totalSecondsInDay = 24 * 3600;
            double intervalSeconds = (_symbols.Count > 0)
                ? (totalSecondsInDay * _symbols.Count) / (double)allowedRequestsPerDay
                : totalSecondsInDay;
            _pollingInterval = TimeSpan.FromSeconds(intervalSeconds);

            _logger.LogInformation("Calculated polling interval per symbol: {Interval} seconds.",
                _pollingInterval.TotalSeconds);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting concurrent polling for symbols via AlphaVantage.");

            // Create a dedicated polling loop for each symbol.
            var pollingTasks = _symbols.Select(symbol => PollSymbolLoop(symbol, stoppingToken));
            await Task.WhenAll(pollingTasks);
        }

        private async Task PollSymbolLoop(string symbol, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting polling loop for symbol {Symbol}.", symbol);

            while (!cancellationToken.IsCancellationRequested)
            {
                await PollAlphaVantageAsync(symbol);

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

        private async Task PollAlphaVantageAsync(string symbol)
        {
            // The GLOBAL_QUOTE function returns the latest quote for a given symbol.
            var url = $"{BaseUrl}?function=GLOBAL_QUOTE&symbol={symbol}&apikey={_apiKey}";
            try
            {
                using var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var rawContent = await response.Content.ReadAsStringAsync();
                    var quoteResponse = System.Text.Json.JsonSerializer.Deserialize<GlobalQuoteResponse>(rawContent);

                    if (quoteResponse?.GlobalQuote != null &&
                        !string.IsNullOrWhiteSpace(quoteResponse.GlobalQuote.Symbol))
                    {
                        var quote = quoteResponse.GlobalQuote;
                        LatestQuotes[symbol] = quote;

                        _logger.LogInformation(
                            "Symbol: {Symbol}, Price: {Price}, Open: {Open}, High: {High}, Low: {Low}, Volume: {Volume}",
                            quote.Symbol, quote.Price, quote.Open, quote.High, quote.Low, quote.Volume);

                        // Push the update via SignalR.
                        await _hubContext.Clients.All.SendAsync("ReceiveQuote", symbol, quote);
                    }
                    else
                    {
                        _logger.LogError(
                            "Received empty or invalid quote data for symbol {Symbol}. Raw response: {RawContent}",
                            symbol, rawContent);
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