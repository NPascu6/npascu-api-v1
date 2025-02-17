using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using npascu_api_v1.Data.Models;

namespace npascu_api_v1.Modules.Auth;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IConfiguration configuration) : ControllerBase
{
    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] UserCredentials credentials)
    {
        var adminUsername = configuration["ADMIN_USERNAME"] ?? "";
        if (adminUsername == null) throw new ArgumentNullException(nameof(adminUsername));

        var adminPassword = configuration["ADMIN_PASSWORD"] ?? "";
        if (adminPassword == null) throw new ArgumentNullException(nameof(adminPassword));

        if (credentials.Username != adminUsername || credentials.Password != adminPassword)
        {
            return Unauthorized();
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(configuration["JWT_KEY"] ?? string.Empty);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new Claim(ClaimTypes.Name, credentials.Username)]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var role = credentials.Username == "admin" ? "Admin" : "User";
        tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }

    [HttpGet("admin-data")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAdminData()
    {
        return Ok(new { Message = "Only admins can access this!" });
    }

    [HttpGet("user-data")]
    [Authorize(Roles = "User")] // Only users with "User" role can access
    public IActionResult GetUserData()
    {
        return Ok(new { Message = "Only users can access this!" });
    }

    [HttpGet("all-data")]
    [Authorize]
    public IActionResult GetAllData()
    {
        return Ok(new { Message = "All authenticated users can access this!" });
    }

    [HttpPost("new-role")]
    [Authorize(Roles = "Admin")]
    public IActionResult AddRole([FromBody] UserCredentials credentials)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(configuration["JWT_KEY"] ?? string.Empty);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new Claim(ClaimTypes.Name, credentials.Username)]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var role = credentials.Username == "admin" ? "Admin" : "User";
        tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }
}