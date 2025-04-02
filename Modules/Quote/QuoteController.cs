using Microsoft.AspNetCore.Mvc;
using npascu_api_v1.Modules.Services.FinnHub;

namespace npascu_api_v1.Modules.Quote
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotesController : ControllerBase
    {
        // GET: api/quotes
        [HttpGet]
        public IActionResult GetQuotes()
        {
            return Ok(FinnHubRestService.LatestQuotes);
        }

        // GET: api/quotes/{symbol}
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
}