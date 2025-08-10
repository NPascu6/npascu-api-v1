using Domain.DTOs;
using Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Discover symbols for an exchange.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Market")]
public class SymbolsController : ControllerBase
{
    private readonly IFinnhubClient _client;
    public SymbolsController(IFinnhubClient client) { _client = client; }

    /// <summary>Returns available symbols for an exchange.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Returns available symbols for an exchange.")]
    [ProducesResponseType(typeof(IEnumerable<FinnhubSymbolDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FinnhubSymbolDto>>> Get([FromQuery] string exchange)
    {
        var symbols = await _client.GetSymbolsAsync(exchange);
        return Ok(symbols);
    }
}
