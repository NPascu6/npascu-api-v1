using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

/// <summary>
/// Hub providing market data streams.
/// </summary>
public class MarketHub : Hub
{
    /// <summary>
    /// Subscribe the current connection to a symbol's updates.
    /// </summary>
    public Task Subscribe(string symbol) => Groups.AddToGroupAsync(Context.ConnectionId, symbol);

    /// <summary>
    /// Unsubscribe the current connection from a symbol's updates.
    /// </summary>
    public Task Unsubscribe(string symbol) => Groups.RemoveFromGroupAsync(Context.ConnectionId, symbol);
}
