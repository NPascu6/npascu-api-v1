using Domain.DTOs;
using Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.RegularExpressions;

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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<FinnhubSymbolDto>>> Get([FromQuery] string exchange)
    {
        var norm = exchange?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(norm) || !Regex.IsMatch(norm, "^[A-Z]+$"))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid exchange",
                Detail = $"Exchange '{exchange}' is invalid.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var symbols = await _client.GetSymbolsAsync(norm);
        return Ok(symbols);
    }
}
