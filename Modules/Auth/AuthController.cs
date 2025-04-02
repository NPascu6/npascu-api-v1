using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using npascu_api_v1.Common.Utils;
using npascu_api_v1.Data;
using npascu_api_v1.Data.Models;
using npascu_api_v1.Modules.DTOs;
using npascu_api_v1.Modules.Services;
using npascu_api_v1.Modules.Services.Token;

namespace npascu_api_v1.Modules.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IConfiguration configuration, ApplicationDbContext context, ITokenService tokenService)
        : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Email already registered.");
            }

            var hashedPassword = PasswordHelper.HashPassword(request.Password);
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = hashedPassword
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await context.Users
                .SingleOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash != null);

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            try
            {
                // Use the new method that verifies the password and generates a token.
                var token = tokenService.AuthenticateAndGenerateToken(user, request.Password, TimeSpan.FromHours(1));
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid credentials");
            }
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var googleClientId = configuration["GOOGLE_CLIENT_ID"];
            if (string.IsNullOrEmpty(googleClientId))
            {
                return Unauthorized("Google Client ID not configured.");
            }

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(
                    request.Token,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new List<string> { googleClientId }
                    }
                );
            }
            catch (Exception)
            {
                return Unauthorized("Invalid Google token.");
            }

            var email = payload.Email;

            var user = await context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                user = new User
                {
                    Name = payload.Name, // or another property as needed.
                    Email = email,
                    PasswordHash = null
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Google users do not have a local password so we directly generate the token.
            var token = tokenService.GenerateToken(user, TimeSpan.FromHours(1));
            return Ok(new { Token = token });
        }
    }
}