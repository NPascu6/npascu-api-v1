using Microsoft.AspNetCore.Mvc;
using npascu_api_v1.Modules.Services.YahooFinance;

namespace npascu_api_v1.Modules.YahooFinance;

[ApiController]
[Route("api/[controller]")]
public class YahooFinanceController : ControllerBase
{
    [HttpGet]
    public IActionResult GetQuotes()
    {
        return Ok(YahooFinanceRestService.LatestQuotes);
    }

    [HttpGet("{symbol}")]
    public IActionResult GetQuote(string symbol)
    {
        if (YahooFinanceRestService.LatestQuotes.TryGetValue(symbol, out var quote))
        {
            return Ok(quote);
        }

        return NotFound();
    }
}
