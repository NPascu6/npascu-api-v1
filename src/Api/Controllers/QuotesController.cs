using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Api.Background;
using Domain.DTOs;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Api.SwaggerExamples;
using System.Linq;

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
    [SwaggerOperation(Summary = "Gets the latest quotes for all tracked symbols.")]
    [ProducesResponseType(typeof(Dictionary<string, SnapshotDto>), StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status200OK, "Quotes keyed by symbol", typeof(Dictionary<string, SnapshotDto>))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(QuotesExample))]
    public IActionResult GetQuotes()
    {
        var mapped = FinnhubRestService.LatestQuotes.ToDictionary(
            kv => kv.Key,
            kv => new SnapshotDto
            {
                Symbol = kv.Key,
                Last = kv.Value.c,
                Bid = 0,
                Ask = 0,
                Open = kv.Value.o,
                High = kv.Value.h,
                Low = kv.Value.l,
                PrevClose = kv.Value.pc,
                Ts = kv.Value.t
            });
        return Ok(mapped);
    }

    /// <summary>
    /// Gets the latest quote for a specified symbol.
    /// </summary>
    /// <param name="symbol">Ticker symbol.</param>
    /// <returns>The latest quote if available.</returns>
    [HttpGet("{symbol}")]
    [SwaggerOperation(Summary = "Gets the latest quote for a specified symbol.")]
    [ProducesResponseType(typeof(SnapshotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status200OK, "Latest quote", typeof(SnapshotDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Quote not found")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(QuoteExample))]
    public IActionResult GetQuote(string symbol)
    {
        if (FinnhubRestService.LatestQuotes.TryGetValue(symbol, out var quote))
        {
            var dto = new SnapshotDto
            {
                Symbol = symbol,
                Last = quote.c,
                Bid = 0,
                Ask = 0,
                Open = quote.o,
                High = quote.h,
                Low = quote.l,
                PrevClose = quote.pc,
                Ts = quote.t
            };
            return Ok(dto);
        }
        return NotFound();
    }
}
