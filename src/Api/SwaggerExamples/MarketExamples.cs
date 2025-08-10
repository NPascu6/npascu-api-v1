using Domain.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace Api.SwaggerExamples;

public class QuotesExample : IExamplesProvider<Dictionary<string, FinnhubQuoteDto>>
{
    public Dictionary<string, FinnhubQuoteDto> GetExamples() => new()
    {
        ["AAPL"] = new FinnhubQuoteDto
        {
            c = 150.12m,
            h = 155.00m,
            l = 149.50m,
            o = 152.30m,
            pc = 151.80m,
            t = 1700000000
        }
    };
}

public class QuoteExample : IExamplesProvider<FinnhubQuoteDto>
{
    public FinnhubQuoteDto GetExamples() => new()
    {
        c = 150.12m,
        h = 155.00m,
        l = 149.50m,
        o = 152.30m,
        pc = 151.80m,
        t = 1700000000
    };
}

public class OrderBookExample : IExamplesProvider<FinnhubOrderBookDto>
{
    public FinnhubOrderBookDto GetExamples() => new()
    {
        s = "AAPL",
        t = 1700000000,
        b = new[] { new[] { 150.00m, 100m } },
        a = new[] { new[] { 150.50m, 80m } }
    };
}

public class TradesExample : IExamplesProvider<FinnhubTradeDto>
{
    public FinnhubTradeDto GetExamples() => new()
    {
        s = "AAPL",
        data = new[]
        {
            new FinnhubTradeTick
            {
                p = 150.10m,
                v = 50m,
                t = 1700000000,
                c = new[] { "@", "T" }
            }
        }
    };
}

