using Domain.DTOs;
using Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Provides order book data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Market")]
public class OrderBookController : ControllerBase
{
    private readonly IFinnhubClient _client;

    public OrderBookController(IFinnhubClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Returns the current order book for a symbol.
    /// </summary>
    /// <param name="symbol">The market symbol.</param>
    /// <param name="depth">Optional depth.</param>
    [HttpGet("{symbol}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FinnhubOrderBookDto>> Get(string symbol, [FromQuery] int? depth)
    {
        var book = await _client.GetOrderBookAsync(symbol, depth);
        if (book == null) return NotFound();
        return Ok(book);
    }
}
