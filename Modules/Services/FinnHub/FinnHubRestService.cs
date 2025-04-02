using System.Collections.Concurrent;
using System.Net;
using Microsoft.AspNetCore.SignalR;
using npascu_api_v1.Modules.DTOs;
using npascu_api_v1.Modules.Quote;

namespace npascu_api_v1.Modules.Services.FinnHub
{
    public class FinnHubRestService : BackgroundService
    {
        private readonly ILogger<FinnHubRestService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IHubContext<QuotesHub> _hubContext;
        private const string BaseUrl = "https://finnhub.io/api/v1/quote";

        private readonly List<string> _symbols;

        public static ConcurrentDictionary<string, FinnhubQuoteDto> LatestQuotes { get; } =
            new();

        public FinnHubRestService(IConfiguration configuration, ILogger<FinnHubRestService> logger,
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
            _logger.LogInformation("Starting sequential round-robin polling for symbols.");
            var symbolIndex = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_symbols.Any())
                {
                    var symbol = _symbols[symbolIndex];
                    await PollFinnhubAsync(symbol);

                    symbolIndex = (symbolIndex + 1) % _symbols.Count;
                }

                // One call per second
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
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

                        _logger.LogInformation(
                            "Symbol: {Symbol}, Price: {Price}, High: {High}, Low: {Low}, Open: {Open}, PrevClose: {PrevClose}, Timestamp: {Timestamp}",
                            symbol, quote.c, quote.h, quote.l, quote.o, quote.pc, quote.t);

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