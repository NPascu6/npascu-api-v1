using Microsoft.IdentityModel.Tokens;
using npascu_api_v1.Repository.Interface;
using npascu_api_v1.Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace npascu_api_v1.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IAuthRepository _authRepository;

        public AuthService(IConfiguration config, IAuthRepository authRepository)
        {
            _config = config;
            _authRepository = authRepository;
        }

        public string Login(string username, string password)
        {
            if (IsUserValid(username, password))
            {
                var token = GenerateJwtToken(username);

                if (token != null)
                {
                    return token;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public string Register(string username, string email, string password)
        {
           
            if(!IsValidEmail(email))
            {
                return null;
            }

            if(_authRepository.RegisterUserAsync(username, email, password) != null)
            {
                return Login(username, password);
            }
            else
            {
                return null;
            }
        }

        private bool IsUserValid(string userName, string password)
        {
            // You should implement your user validation logic here, e.g., querying a database
            // For this example, a basic hard-coded validation is used
            var user = _authRepository.LoginUserAsync(userName, password);

            if(user != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GenerateJwtToken(string userName)
        {
            var configSecretKey = _config.GetSection("Authentication").GetSection("Swagger").GetSection("Secret").Value;
            var configIssuer = _config.GetSection("Authentication").GetSection("Swagger").GetSection("Issuer").Value;
            var configAudience = _config.GetSection("Authentication").GetSection("Swagger").GetSection("Audience").Value;

            if(configSecretKey == null || configIssuer == null || configAudience == null)
            {
                return null;
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configSecretKey));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName)
            };

            var tokenOptions = new JwtSecurityToken(
                issuer: configIssuer,
                audience: configAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30), // Adjust the token expiration as needed
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return tokenString;
        }

        private bool IsValidEmail(string email)
        {
            // Use a regular expression to check if the email is in a valid format
            // This is a basic example; you can use more comprehensive patterns for email validation
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            return Regex.IsMatch(email, emailPattern);
        }
    }
}
