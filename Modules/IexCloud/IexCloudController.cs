using Microsoft.AspNetCore.Mvc;
using npascu_api_v1.Modules.Services.IexCloud;

namespace npascu_api_v1.Modules.IexCloud;

[ApiController]
[Route("api/[controller]")]
public class IexCloudController : ControllerBase
{
    [HttpGet]
    public IActionResult GetQuotes()
    {
        return Ok(IexCloudRestService.LatestQuotes);
    }

    [HttpGet("{symbol}")]
    public IActionResult GetQuote(string symbol)
    {
        if (IexCloudRestService.LatestQuotes.TryGetValue(symbol, out var quote))
        {
            return Ok(quote);
        }

        return NotFound();
    }
}
