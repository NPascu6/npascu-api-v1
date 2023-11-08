using Microsoft.IdentityModel.Tokens;
using npascu_api_v1.Repository.Interface;
using npascu_api_v1.Services.Email.Interface;
using npascu_api_v1.Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace npascu_api_v1.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private ILogger<AuthService> _logger;
        private readonly IConfiguration _config;
        private readonly IAuthRepository _authRepository;
        private readonly IEmailService _emailService;

        public AuthService(IConfiguration config, IAuthRepository authRepository, IEmailService emailService, ILogger<AuthService> logger)
        {
            _config = config;
            _authRepository = authRepository;
            _emailService = emailService;
            _logger = logger;
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
                    return "";
                }
            }
            else
            {
                return null;
            }
        }

        public string Register(string username, string email, string password)
        {

            if (!IsValidEmail(email))
            {
                return null;
            }

            if (_authRepository.RegisterUserAsync(username, email, password) != null)
            {
                var sentRegistrationEmail = SendEmailVerification(email);
                _logger.LogInformation("Sent registration email: " + sentRegistrationEmail);

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

            if (user != null)
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
            try
            {
                var configSecretKey = _config.GetSection("Authentication").GetSection("Swagger").GetSection("Secret").Value;
                var configIssuer = _config.GetSection("Authentication").GetSection("Swagger").GetSection("Issuer").Value;
                var configAudience = _config.GetSection("Authentication").GetSection("Swagger").GetSection("Audience").Value;

                if (configSecretKey == null || configIssuer == null || configAudience == null)
                {
                    return "";
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
            catch (Exception ex)
            {

                throw new Exception("An error occurred while generating a JWT token.", ex);
            }
        }

        private bool IsValidEmail(string email)
        {
            // Use a regular expression to check if the email is in a valid format
            // This is a basic example; you can use more comprehensive patterns for email validation
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            return Regex.IsMatch(email, emailPattern);
        }

        private bool SendEmailVerification(string email)
        {
            try
            {
                var registrationToken = GenerateRegistrationToken(); // Generate a registration token.
                var emailVerificationPath = _config.GetSection("Authentication").GetSection("Swagger").GetSection("EmailVerificationPath").Value;

                // Construct the email content with a link containing the registration token.
                var emailContent = $@"Thank you for registering to my personal .Net Core API! To complete your registration and ensure your account remains active, please click the following link to verify your email within the next 24 hours:

                {emailVerificationPath}{registrationToken}

                If you have any questions or need assistance, feel free to contact our support team(me) by replying to this email. Welcome to our community!";

                // Send the email to the user's email address.
                var response = _emailService.SendEmailAsync(email, "Email Verification", emailContent).Result;

                if (!response)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while sending the email verification.", ex);
            }

        }

        private string GenerateRegistrationToken()
        {
            // Generate a random token using a secure random number generator or a cryptographic library.
            // For simplicity, this example uses a GUID (Globally Unique Identifier) as a token.
            // You should use a more secure method for generating tokens in a production environment.

            Guid registrationToken = Guid.NewGuid();
            return registrationToken.ToString();
        }
    }
}
