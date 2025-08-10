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
/// Provides quote snapshots.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Market")]
public class SnapshotController : ControllerBase
{
    private readonly IFinnhubClient _client;

    public SnapshotController(IFinnhubClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Returns the latest quote snapshot.
    /// </summary>
    /// <param name="symbol">The market symbol.</param>
    [HttpGet("{symbol}")]
    [SwaggerOperation(Summary = "Returns the latest quote snapshot.")]
    [ProducesResponseType(typeof(SnapshotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status200OK, "Quote snapshot", typeof(SnapshotDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Snapshot not found")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(QuoteExample))]
    public async Task<ActionResult<SnapshotDto>> Get(string symbol)
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
        var snap = await _client.GetQuoteAsync(norm);
        if (snap == null) return NotFound();

        var book = await _client.GetOrderBookAsync(norm, 1);
        decimal bid = book?.b?.FirstOrDefault()?[0] ?? 0m;
        decimal ask = book?.a?.FirstOrDefault()?[0] ?? 0m;

        var dto = new SnapshotDto
        {
            Symbol = norm,
            Last = snap.c,
            Bid = bid,
            Ask = ask,
            Open = snap.o,
            High = snap.h,
            Low = snap.l,
            PrevClose = snap.pc,
            Ts = snap.t
        };
        return Ok(dto);
    }
}
