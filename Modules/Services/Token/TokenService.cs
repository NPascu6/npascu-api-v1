using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using npascu_api_v1.Common.Utils;
using npascu_api_v1.Data.Models;

namespace npascu_api_v1.Modules.Services.Token
{
    public class TokenService(IConfiguration configuration) : ITokenService
    {
        public string GenerateToken(User user, TimeSpan? expiration = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["JWT_KEY"] ?? string.Empty);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email),
                user.Name.Equals("admin", StringComparison.InvariantCultureIgnoreCase)
                    ? new Claim(ClaimTypes.Role, "Admin")
                    : new Claim(ClaimTypes.Role, "User")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(expiration ?? TimeSpan.FromHours(1)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string AuthenticateAndGenerateToken(User user, string providedPassword, TimeSpan? expiration = null)
        {
            if (string.IsNullOrEmpty(user.PasswordHash) ||
                !PasswordHelper.VerifyPassword(providedPassword, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid password.");
            }

            return GenerateToken(user, expiration);
        }
    }
}