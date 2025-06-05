using Microsoft.AspNetCore.Mvc;
using npascu_api_v1.Modules.Services.AlphaVantage;
using npascu_api_v1.Modules.Services.FinnHub;

namespace npascu_api_v1.Modules.Quote
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotesController : ControllerBase
    {
        private readonly AlphaVantageHistoricalService _historicalService;

        public QuotesController(AlphaVantageHistoricalService historicalService)
        {
            _historicalService = historicalService;
        }

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

        // GET: api/quotes/historical/{symbol}
        [HttpGet("historical/{symbol}")]
        public async Task<IActionResult> GetHistorical(string symbol)
        {
            var data = await _historicalService.GetDailyHistoryAsync(symbol);
            if (data != null)
            {
                return Ok(data);
            }

            return NotFound();
        }
    }
}
