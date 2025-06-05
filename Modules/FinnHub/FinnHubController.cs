using Microsoft.AspNetCore.Mvc;
using npascu_api_v1.Modules.Services.FinnHub;

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

    [HttpGet("{symbol}")]
    public IActionResult GetQuote(string symbol)
    {
        if (FinnHubRestService.LatestQuotes.TryGetValue(symbol, out var quote))
        {
            return Ok(quote);
        }

        return NotFound();
    }
}
