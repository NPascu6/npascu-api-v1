using Domain.DTOs;

namespace Infrastructure.Clients;

public interface IFinnhubClient
{
    Task<FinnhubOrderBookDto?> GetOrderBookAsync(string symbol, int? depth = null, CancellationToken ct = default);
    Task<FinnhubTradeDto?> GetTradesAsync(string symbol, DateTime from, DateTime to, int? limit = null, CancellationToken ct = default);
    Task<FinnhubQuoteDto?> GetQuoteAsync(string symbol, CancellationToken ct = default);
}
