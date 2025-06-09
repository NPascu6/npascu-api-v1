using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Swagger;

namespace npascu_api_v1.Modules.OpenApi;

[ApiController]
[Route("api/[controller]")]
public class OpenApiController(ISwaggerProvider swaggerProvider) : ControllerBase
{
    [HttpGet]
    public IActionResult GetDefinition()
    {
        var document = swaggerProvider.GetSwagger("v1");
        return Ok(document);
    }
}
