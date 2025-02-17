using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using npascu_api_v1.Data;
using npascu_api_v1.Data.Models;
using System.Security.Cryptography;

namespace npascu_api_v1.Modules.Auth
{
    public class UserCredentialsController(ApplicationDbContext context, IConfiguration configuration) : ControllerBase
    {
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var enteredHash = HashPassword(enteredPassword);
            return enteredHash == storedHash;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCredentials credentials)
        {
            var hashedPassword = HashPassword(credentials.Password);
            var user = new User
            {
                PasswordHash = hashedPassword,
                Name = credentials.Username,
                Email = credentials.Email
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserCredentials credentials)
        {
            var user = context.Users.SingleOrDefault(u => u.Name == credentials.Username);
            if (user == null || !VerifyPassword(credentials.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["JWT_KEY"] ?? string.Empty);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([new Claim(ClaimTypes.Name, user.Name)]),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }
    }
}