using Domain.DTOs;
using Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;
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
    [ProducesResponseType(typeof(FinnhubQuoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status200OK, "Quote snapshot", typeof(FinnhubQuoteDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Snapshot not found")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(QuoteExample))]
    public async Task<ActionResult<FinnhubQuoteDto>> Get(string symbol)
    {
        var snap = await _client.GetQuoteAsync(symbol);
        if (snap == null) return NotFound();
        return Ok(snap);
    }
}
