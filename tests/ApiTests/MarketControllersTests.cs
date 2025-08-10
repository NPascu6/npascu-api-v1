using Api.Controllers;
using Domain.DTOs;
using Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

class FakeFinnhubClient : IFinnhubClient
{
    public Task<FinnhubOrderBookDto?> GetOrderBookAsync(string symbol, int? depth = null, CancellationToken ct = default)
        => Task.FromResult<FinnhubOrderBookDto?>(new FinnhubOrderBookDto
        {
            t = 1,
            b = new[] { new[] { 1m, 1m } },
            a = new[] { new[] { 2m, 1m } }
        });
    public Task<FinnhubTradeDto?> GetTradesAsync(string symbol, DateTime from, DateTime to, int? limit = null, CancellationToken ct = default)
        => Task.FromResult<FinnhubTradeDto?>(new FinnhubTradeDto());
    public Task<FinnhubQuoteDto?> GetQuoteAsync(string symbol, CancellationToken ct = default)
        => Task.FromResult<FinnhubQuoteDto?>(new FinnhubQuoteDto());
    public Task<FinnhubCandleDto?> GetCandlesAsync(string symbol, string resolution, DateTime from, DateTime to, CancellationToken ct = default)
        => Task.FromResult<FinnhubCandleDto?>(new FinnhubCandleDto());
    public Task<FinnhubCompanyProfileDto?> GetProfileAsync(string symbol, CancellationToken ct = default)
        => Task.FromResult<FinnhubCompanyProfileDto?>(new FinnhubCompanyProfileDto());
    public Task<IEnumerable<FinnhubSymbolDto>> GetSymbolsAsync(string exchange, CancellationToken ct = default)
        => Task.FromResult<IEnumerable<FinnhubSymbolDto>>(Array.Empty<FinnhubSymbolDto>());
}

public class MarketControllersTests
{
    [Fact]
    public async Task OrderBook_ReturnsOk()
    {
        var controller = new OrderBookController(new FakeFinnhubClient());
        var result = await controller.Get("AAPL", null);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<OrderBookDto>(ok.Value);
    }
}
