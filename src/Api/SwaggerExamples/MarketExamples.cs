using Domain.DTOs;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

namespace Api.SwaggerExamples;

public class QuotesExample : IExamplesProvider<Dictionary<string, SnapshotDto>>
{
    public Dictionary<string, SnapshotDto> GetExamples() => new()
    {
        ["AAPL"] = new SnapshotDto
        {
            Symbol = "AAPL",
            Last = 150.12m,
            Bid = 150.00m,
            Ask = 150.20m,
            Open = 152.30m,
            High = 155.00m,
            Low = 149.50m,
            PrevClose = 151.80m,
            Ts = 1700000000
        }
    };
}

public class QuoteExample : IExamplesProvider<SnapshotDto>
{
    public SnapshotDto GetExamples() => new()
    {
        Symbol = "AAPL",
        Last = 150.12m,
        Bid = 150.00m,
        Ask = 150.20m,
        Open = 152.30m,
        High = 155.00m,
        Low = 149.50m,
        PrevClose = 151.80m,
        Ts = 1700000000
    };
}

public class OrderBookExample : IExamplesProvider<OrderBookDto>
{
    public OrderBookDto GetExamples() => new()
    {
        Symbol = "AAPL",
        Ts = 1700000000,
        Bids = new() { new Level { Price = 150.00m, Size = 100m } },
        Asks = new() { new Level { Price = 150.50m, Size = 80m } },
        Depth = 1
    };
}

public class TradesExample : IExamplesProvider<IEnumerable<TradeDto>>
{
    public IEnumerable<TradeDto> GetExamples() => new[]
    {
        new TradeDto
        {
            Symbol = "AAPL",
            Price = 150.10m,
            Size = 50m,
            Ts = 1700000000
        }
    };
}

