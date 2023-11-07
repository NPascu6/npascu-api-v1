using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using npascu_api_v1.Models.Entities.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace npascu_api_v1.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost, Route("login")]
        public IActionResult Login(LoginModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid client request");
            }
            if (model.UserName == "test" && model.Password == "1234")
            {

                var configSecretKey = _config.GetSection("Authentication").GetSection("Swagger").GetSection("Secret").Value;
                var configIssuer = _config.GetSection("Authentication").GetSection("Swagger").GetSection("Issuer").Value;
                var configAudience = _config.GetSection("Authentication").GetSection("Swagger").GetSection("Audience").Value;

                try
                {
                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configSecretKey));
                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                    var tokenOptions = new JwtSecurityToken(
                        issuer: configIssuer,
                        audience: configAudience,
                        claims: new List<Claim>(),
                        expires: DateTime.Now.AddMinutes(5),
                        signingCredentials: signinCredentials
                    );
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                    return Ok(new { Token = tokenString });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
