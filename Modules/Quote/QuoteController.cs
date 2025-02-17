using Microsoft.AspNetCore.Mvc;
using npascu_api_v1.Modules.Background;

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
            // Return the latest quotes as a JSON object.
            return Ok(FinnhubRestService.LatestQuotes);
        }

        // GET: api/quotes/{symbol}
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
}