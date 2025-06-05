using Microsoft.AspNetCore.Mvc;
using npascu_api_v1.Modules.Services.Polygon;

namespace npascu_api_v1.Modules.Polygon;

[ApiController]
[Route("api/[controller]")]
public class PolygonController : ControllerBase
{
    [HttpGet]
    public IActionResult GetQuotes()
    {
        return Ok(PolygonRestService.LatestQuotes);
    }

    [HttpGet("{symbol}")]
    public IActionResult GetQuote(string symbol)
    {
        if (PolygonRestService.LatestQuotes.TryGetValue(symbol, out var quote))
        {
            return Ok(quote);
        }

        return NotFound();
    }
}
