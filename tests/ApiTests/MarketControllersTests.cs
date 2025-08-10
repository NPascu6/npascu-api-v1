using Api.Controllers;
using Domain.DTOs;
using Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;
using System;
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
        var result = await controller.Get("NASDAQ:AAPL", null);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<OrderBookDto>(ok.Value);
    }

    [Fact]
    public async Task OrderBook_InvalidSymbol_ReturnsBadRequest()
    {
        var controller = new OrderBookController(new FakeFinnhubClient());
        var result = await controller.Get("AAPL", null);
        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<ProblemDetails>(bad.Value);
    }

    [Fact]
    public async Task Trades_InvalidSymbol_ReturnsBadRequest()
    {
        var controller = new TradesController(new FakeFinnhubClient());
        var from = DateTime.UtcNow.AddMinutes(-1);
        var to = DateTime.UtcNow;
        var result = await controller.Get("AAPL", from, to, null);
        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<ProblemDetails>(bad.Value);
    }

    [Fact]
    public async Task Snapshot_InvalidSymbol_ReturnsBadRequest()
    {
        var controller = new SnapshotController(new FakeFinnhubClient());
        var result = await controller.Get("AAPL");
        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<ProblemDetails>(bad.Value);
    }

    [Fact]
    public async Task Candles_InvalidSymbol_ReturnsBadRequest()
    {
        var controller = new CandlesController(new FakeFinnhubClient());
        var from = DateTime.UtcNow.AddMinutes(-10);
        var to = DateTime.UtcNow;
        var result = await controller.Get("AAPL", "1", from, to);
        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<ProblemDetails>(bad.Value);
    }

    [Fact]
    public async Task Profile_InvalidSymbol_ReturnsBadRequest()
    {
        var controller = new ProfileController(new FakeFinnhubClient());
        var result = await controller.Get("AAPL");
        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<ProblemDetails>(bad.Value);
    }

    [Fact]
    public void Quote_InvalidSymbol_ReturnsBadRequest()
    {
        var controller = new QuotesController();
        var result = controller.GetQuote("AAPL");
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<ProblemDetails>(bad.Value);
    }
}
