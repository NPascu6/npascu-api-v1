using Domain.DTOs;
using Domain.Services;
using Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Market")]
public class ProfileController : ControllerBase
{
    private readonly IFinnhubClient _client;
    public ProfileController(IFinnhubClient client) { _client = client; }

    /// <summary>Returns basic company profile data.</summary>
    [HttpGet("{symbol}")]
    [SwaggerOperation(Summary = "Returns basic company profile data.")]
    [ProducesResponseType(typeof(FinnhubCompanyProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FinnhubCompanyProfileDto>> Get(string symbol)
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
        var profile = await _client.GetProfileAsync(norm);
        if (profile == null) return NotFound();
        return Ok(profile);
    }
}
