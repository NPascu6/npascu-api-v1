using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Api.Background;

namespace Api.Controllers;

/// <summary>
/// Provides market quote endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Quotes")]
public class QuotesController : ControllerBase
{
    /// <summary>
    /// Gets the latest quotes for all tracked symbols.
    /// </summary>
    /// <returns>A collection of quotes keyed by symbol.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetQuotes()
    {
        return Ok(FinnhubRestService.LatestQuotes);
    }

    /// <summary>
    /// Gets the latest quote for a specified symbol.
    /// </summary>
    /// <param name="symbol">Ticker symbol.</param>
    /// <returns>The latest quote if available.</returns>
    [HttpGet("{symbol}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetQuote(string symbol)
    {
        if (FinnhubRestService.LatestQuotes.TryGetValue(symbol, out var quote))
        {
            return Ok(quote);
        }
        return NotFound();
    }
}
