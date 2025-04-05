using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using npascu_api_v1.Modules.DTOs;
using npascu_api_v1.Modules.Quote;

namespace npascu_api_v1.Modules.Services.FinnHub;

public class FinnHubWebSocketService : BackgroundService
{
    private readonly ILogger<FinnHubWebSocketService> _logger;
    private readonly IHubContext<QuotesHub> _hubContext;
    private readonly List<string> _symbols;
    private readonly ClientWebSocket _webSocket = new();
    private readonly Uri _socketUri;
    private static readonly ConcurrentDictionary<string, FinnhubTradeDto> LatestTrades = new();
    private static readonly TimeSpan PublishInterval = TimeSpan.FromSeconds(1);
    private Task _publishTask = Task.CompletedTask;

    public FinnHubWebSocketService(
        IConfiguration configuration,
        ILogger<FinnHubWebSocketService> logger,
        IHubContext<QuotesHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;

        var apiKey = configuration["FINNHUB_API_KEY"]
                     ?? throw new InvalidOperationException("Finnhub API key not configured.");

        _symbols = configuration["FINNHUB_SYMBOLS"]?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList() ?? new List<string>();

        _socketUri = new Uri($"wss://ws.finnhub.io?token={apiKey}");
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
                await SendMessageAsync(JsonSerializer.Serialize(new { type = "subscribe", symbol }), stoppingToken);
                _logger.LogInformation("Subscribed to symbol {Symbol}", symbol);
            }

            await Task.WhenAll(
                ReceiveMessagesLoop(stoppingToken),
                PublishUpdatesLoop(stoppingToken)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WebSocket connection error.");
        }
    }

    private async Task SendMessageAsync(string message, CancellationToken cancellationToken)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        await _webSocket.SendAsync(
            new ArraySegment<byte>(messageBytes),
            WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken
        );
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
                    _logger.LogInformation("WebSocket closed by server.");
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server",
                        cancellationToken);
                    break;
                }

                if (result.Count > 0)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    ProcessMessage(message);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error receiving WebSocket message.");
            }
        }
    }

    private void ProcessMessage(string message)
    {
        try
        {
            var response = JsonSerializer.Deserialize<FinnhubWebSocketResponse>(message);

            if (response is { Type: "trade", Data: not null })
            {
                foreach (var trade in response.Data)
                {
                    LatestTrades[trade.Symbol] = trade;
                    _logger.LogInformation("Received trade for {Symbol}: Price {Price} at {Timestamp}",
                        trade.Symbol, trade.Price, trade.Timestamp);
                }
            }
            else
            {
                _logger.LogWarning("Received unexpected message type: {Type}. Raw: {Message}", response?.Type, message);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize incoming message: {Message}", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing message: {Message}", message);
        }
    }

    private async Task PublishUpdatesLoop(CancellationToken cancellationToken)
    {
        async Task PublishPeriodicallyAsync()
        {
            try
            {
                await Task.Delay(PublishInterval, cancellationToken);

                if (!LatestTrades.IsEmpty)
                {
                    foreach (var (symbol, trade) in LatestTrades)
                    {
                        await _hubContext.Clients.All.SendAsync("ReceiveTrade", symbol, trade, cancellationToken);
                    }
                }

                _publishTask = PublishPeriodicallyAsync();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Publishing updates canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing updates.");
            }
        }

        _publishTask = PublishPeriodicallyAsync();
        await _publishTask;
    }


    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Finnhub WebSocket Service.");

        if (_webSocket.State == WebSocketState.Open || _webSocket.State == WebSocketState.CloseReceived)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Service stopping", cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }
}