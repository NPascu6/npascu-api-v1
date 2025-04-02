using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using npascu_api_v1.Modules.DTOs;
using npascu_api_v1.Modules.Quote;

namespace npascu_api_v1.Modules.Services.FinnHub
{
    public class FinnHubWebSocketService : BackgroundService
    {
        private readonly ILogger<FinnHubWebSocketService> _logger;
        private readonly IHubContext<QuotesHub> _hubContext;
        private readonly List<string> _symbols;
        private readonly ClientWebSocket _webSocket;
        private readonly Uri _socketUri;

        private static ConcurrentDictionary<string, FinnhubTradeDto> LatestTrades { get; } =
            new();

        private readonly TimeSpan _publishInterval = TimeSpan.FromSeconds(1);

        public FinnHubWebSocketService(IConfiguration configuration,
            ILogger<FinnHubWebSocketService> logger,
            IHubContext<QuotesHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
            var apiKey = configuration["FINNHUB_API_KEY"] ?? throw new Exception("Finhub API key not configured.");

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
                _symbols = ["AAPL", "MSFT", "GOOGL"];
            }

            _socketUri = new Uri($"wss://ws.finnhub.io?token={apiKey}");
            _webSocket = new ClientWebSocket();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting Finnhub WebSocket Service.");

            try
            {
                await _webSocket.ConnectAsync(_socketUri, stoppingToken);
                _logger.LogInformation("Connected to Finnhub WebSocket.");

                foreach (var symbol in _symbols)
                {
                    var subscribeMessage = new { type = "subscribe", symbol };
                    var messageJson = JsonSerializer.Serialize(subscribeMessage);
                    await SendMessageAsync(messageJson, stoppingToken);
                    _logger.LogInformation("Subscribed to symbol {Symbol}", symbol);
                }

                var receiverTask = ReceiveMessagesLoop(stoppingToken);
                var publisherTask = PublishUpdatesLoop(stoppingToken);

                await Task.WhenAll(receiverTask, publisherTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WebSocket connection error.");
            }
        }

        private async Task SendMessageAsync(string message, CancellationToken cancellationToken)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(messageBytes);
            await _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, cancellationToken);
        }

        private async Task ReceiveMessagesLoop(CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];

            while (!cancellationToken.IsCancellationRequested && _webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _logger.LogInformation("WebSocket closed by the server.");
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server",
                            cancellationToken);
                        break;
                    }

                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    ProcessMessage(message);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while receiving WebSocket message.");
                }
            }
        }

        /// <summary>
        /// Processes an incoming message by deserializing it and updating the cache.
        /// </summary>
        private void ProcessMessage(string message)
        {
            try
            {
                // Finnhub sends messages with the following structure:
                // {
                //    "data": [
                //       {
                //          "p": 261.74,      // Price
                //          "s": "AAPL",      // Symbol
                //          "t": 1582641008,  // Timestamp (epoch)
                //          "v": 100          // Volume
                //       }
                //    ],
                //    "type": "trade"
                // }
                var response = JsonSerializer.Deserialize<FinnhubWebSocketResponse>(message);
                if (response != null && response.Type == "trade" && response.Data != null)
                {
                    foreach (var trade in response.Data)
                    {
                        // Update the cache with the latest trade for each symbol.
                        LatestTrades[trade.Symbol] = trade;

                        _logger.LogInformation("Received trade for {Symbol}: Price {Price} at {Timestamp}",
                            trade.Symbol, trade.Price, trade.Timestamp);
                    }
                }
                else
                {
                    _logger.LogWarning("Received message of type {Type} or empty data: {Message}", response?.Type,
                        message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message: {Message}", message);
            }
        }

        /// <summary>
        /// Publishes aggregated updates to SignalR at a controlled interval.
        /// </summary>
        private async Task PublishUpdatesLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_publishInterval, cancellationToken);

                    foreach (var kvp in LatestTrades)
                    {
                        await _hubContext.Clients.All.SendAsync("ReceiveTrade", kvp.Key, kvp.Value, cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while publishing aggregated updates.");
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Finnhub WebSocket Service.");
            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Service stopping", cancellationToken);
            }

            await base.StopAsync(cancellationToken);
        }
    }
}