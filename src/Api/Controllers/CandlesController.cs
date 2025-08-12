using Domain.DTOs;
using Domain.Services;
using Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandlesController : ControllerBase
{
    private readonly IFinnhubClient _client;
    public CandlesController(IFinnhubClient client) { _client = client; }

    /// <summary>Returns historical candlesticks.</summary>
    [HttpGet("{symbol}")]
    [SwaggerOperation(Summary = "Returns historical candlesticks.")]
    [ProducesResponseType(typeof(FinnhubCandleDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FinnhubCandleDto>> Get(string symbol, [FromQuery] string resolution, [FromQuery] DateTime from, [FromQuery] DateTime to)
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
        var candles = await _client.GetCandlesAsync(norm, resolution, from, to);
        if (candles == null) return NotFound();
        return Ok(candles);
    }
}
