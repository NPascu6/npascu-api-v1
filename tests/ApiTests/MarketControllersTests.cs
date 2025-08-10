using Api.Controllers;
using Domain.DTOs;
using Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

class FakeFinnhubClient : IFinnhubClient
{
    public Task<FinnhubOrderBookDto?> GetOrderBookAsync(string symbol, int? depth = null, CancellationToken ct = default)
        => Task.FromResult<FinnhubOrderBookDto?>(new FinnhubOrderBookDto());
    public Task<FinnhubTradeDto?> GetTradesAsync(string symbol, DateTime from, DateTime to, int? limit = null, CancellationToken ct = default)
        => Task.FromResult<FinnhubTradeDto?>(new FinnhubTradeDto());
    public Task<FinnhubQuoteDto?> GetQuoteAsync(string symbol, CancellationToken ct = default)
        => Task.FromResult<FinnhubQuoteDto?>(new FinnhubQuoteDto());
}

public class MarketControllersTests
{
    [Fact]
    public async Task OrderBook_ReturnsOk()
    {
        var controller = new OrderBookController(new FakeFinnhubClient());
        var result = await controller.Get("AAPL", null);
        Assert.IsType<OkObjectResult>(result.Result);
    }
}
