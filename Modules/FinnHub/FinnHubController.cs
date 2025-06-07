using Microsoft.AspNetCore.Mvc;
using npascu_api_v1.Modules.Services.FinnHub;
using System.Text.Json;

namespace npascu_api_v1.Modules.FinnHub;

[ApiController]
[Route("api/[controller]")]
public class FinnHubController : ControllerBase
{
    [HttpGet]
    public IActionResult GetQuotes()
    {
        return Ok(FinnHubRestService.LatestQuotes);
    }

    [HttpGet("tickers")]
    public IActionResult GetTickers()
    {
        return Ok(FinnHubRestService.Symbols);
    }

    [HttpGet("{symbol}")]
    public IActionResult GetQuote(string symbol)
    {
        if (FinnHubRestService.LatestQuotes.TryGetValue(symbol, out var quote))
        {
            return Ok(quote);
        }

        return NotFound();
    }

    [HttpGet("stream")]
    public async Task StreamQuotes(CancellationToken cancellationToken)
    {
        Response.Headers.Append("Content-Type", "text/event-stream");

        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var (symbol, quote) in FinnHubRestService.LatestQuotes)
            {
                var json = JsonSerializer.Serialize(new { symbol, quote });
                await Response.WriteAsync($"data: {json}\n\n", cancellationToken);
            }

            await Response.Body.FlushAsync(cancellationToken);

            try
            {
                await Task.Delay(1000, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
