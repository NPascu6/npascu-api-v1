using Domain.DTOs;
using Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Provides recent trade data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Market")]
public class TradesController : ControllerBase
{
    private readonly IFinnhubClient _client;

    public TradesController(IFinnhubClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Returns trades for a symbol within a time range.
    /// </summary>
    /// <param name="symbol">The market symbol.</param>
    /// <param name="from">Start time in UTC.</param>
    /// <param name="to">End time in UTC.</param>
    /// <param name="limit">Optional limit.</param>
    [HttpGet("{symbol}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FinnhubTradeDto>> Get(string symbol, [FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int? limit)
    {
        if (from == default || to == default)
            return BadRequest("from and to are required");

        var trades = await _client.GetTradesAsync(symbol, from, to, limit);
        if (trades == null) return NotFound();
        return Ok(trades);
    }
}
