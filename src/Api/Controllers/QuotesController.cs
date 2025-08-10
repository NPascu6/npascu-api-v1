using Microsoft.AspNetCore.Mvc;
using Api.Background;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuotesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetQuotes()
    {
        return Ok(FinnhubRestService.LatestQuotes);
    }

    [HttpGet("{symbol}")]
    public IActionResult GetQuote(string symbol)
    {
        if (FinnhubRestService.LatestQuotes.TryGetValue(symbol, out var quote))
        {
            return Ok(quote);
        }
        return NotFound();
    }
}
