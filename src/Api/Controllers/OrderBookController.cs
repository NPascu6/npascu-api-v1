using Domain.DTOs;
using Domain.Services;
using Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Api.SwaggerExamples;

namespace Api.Controllers;

/// <summary>
/// Provides order book data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
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
    [SwaggerOperation(Summary = "Returns the current order book for a symbol.")]
    [ProducesResponseType(typeof(OrderBookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status200OK, "Order book snapshot", typeof(OrderBookDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Book not found")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OrderBookExample))]
    public async Task<ActionResult<OrderBookDto>> Get(string symbol, [FromQuery] int? depth)
    {
        var norm = SymbolNormalizer.Normalize(symbol);
        if (!SymbolNormalizer.IsValid(norm))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid symbol",
                Detail = $"Symbol '{symbol}' is invalid.",
                Status = StatusCodes.Status400BadRequest
            });
        }
        var book = await _client.GetOrderBookAsync(norm, depth);
        if (book == null) return NotFound();

        var bids = book.b
            .Where(l => l.Length >= 2 && l[1] > 0)
            .Select(l => new Level { Price = l[0], Size = l[1] })
            .OrderByDescending(l => l.Price)
            .ToList();
        var asks = book.a
            .Where(l => l.Length >= 2 && l[1] > 0)
            .Select(l => new Level { Price = l[0], Size = l[1] })
            .OrderBy(l => l.Price)
            .ToList();

        var dto = new OrderBookDto
        {
            Symbol = norm,
            Ts = book.t,
            Bids = bids,
            Asks = asks,
            Depth = Math.Max(bids.Count, asks.Count)
        };
        return Ok(dto);
    }
}
