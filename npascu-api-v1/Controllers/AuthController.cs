using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using npascu_api_v1.Models.Entities.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace npascu_api_v1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;

        public AuthController(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _configuration = configuration;
            _environment = hostEnvironment;
        }


        [HttpPost, Route("login")]
        public IActionResult Login(LoginModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid client request");
            }
            if (model.UserName == "test" && model.Password == "test")
            {
   

                if (_environment.IsDevelopment())
                {
                    var section = _configuration.GetSection("JwtSettingsDev");
                    var jwtIssuer = section["Issuer"];
                    var jwtAudience = section["Audience"];
                    var jwtSecret = section["SecretKey"];

                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                    var tokenOptions = new JwtSecurityToken(
                        issuer: jwtIssuer,
                        audience: jwtAudience,
                        claims: new List<Claim>(),
                        expires: DateTime.Now.AddMinutes(5),
                        signingCredentials: signinCredentials
                    );
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                    return Ok(new { Token = tokenString });
                }
                else
                {
                    var section = _configuration.GetSection("JwtSettingsProd");
                    var jwtIssuer = section["Issuer"];
                    var jwtAudience = section["Audience"];
                    var jwtSecret = section["SecretKey"];

                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                    var tokenOptions = new JwtSecurityToken(
                        issuer: jwtIssuer,
                        audience: jwtAudience,
                        claims: new List<Claim>(),
                        expires: DateTime.Now.AddMinutes(5),
                        signingCredentials: signinCredentials
                    );
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                    return Ok(new { Token = tokenString });
                }

                
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
