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
                _logger.LogInformation("User is valid: " + username);
                var token = GenerateJwtToken(username);

                if (token != null)
                {
                    _logger.LogInformation("Token generated: " + token);
                    return token;
                }
                else
                {
                    _logger.LogInformation("Token not generated: " + token);
                    return "";
                }
            }
            else
            {
                _logger.LogInformation("User is not valid: " + username);
                return null;
            }
        }

        public string Register(string username, string email, string password)
        {

            if (!IsValidEmail(email))
            {
                return null;
            }
            var registrationToken = GenerateRegistrationToken();
            _logger.LogInformation("Registration token generated: " + registrationToken);

            var user = _authRepository.RegisterUserAsync(username, email, password, registrationToken);
            _logger.LogInformation("User registered: " + user);

            if (user != null)
            {

                var sentRegistrationEmail = SendEmailVerification(email, registrationToken);
                _logger.LogInformation("Sent registration email: " + sentRegistrationEmail);

                _logger.LogInformation("Loging in: " + user);
                return Login(username, password);
            }
            else
            {
                _logger.LogInformation("User not registered: " + user);
                return null;
            }
        }

        public bool ValidateEmail(string token)
        {
            // You should implement your email validation logic here, e.g., querying a database
            // For this example, a basic hard-coded validation is used
            if (token == null)
            {
                return false;
            }

            _logger.LogInformation("Validating email: " + token);
            var isValid = _authRepository.ValidateEmail(token);

            if (isValid)
            {
                _logger.LogInformation("Email validated: " + token);
                return true;
            }
            else
            {
                _logger.LogInformation("Email not validated: " + token);
                return false;
            }
        }

        public bool DeleteUser(string username)
        {
            if (username == null)
            {
                return false;
            }

            _logger.LogInformation("Deleting user: " + username);
            var deleted = _authRepository.DeleteUser(username);

            if (deleted)
            {
                _logger.LogInformation("User deleted: " + username);
                return true;
            }
            else
            {
                _logger.LogInformation("User not deleted: " + username);
                return false;
            }
        }

        private bool IsUserValid(string userName, string password)
        {
            // You should implement your user validation logic here, e.g., querying a database
            // For this example, a basic hard-coded validation is used
            var user = _authRepository.LoginUserAsync(userName, password);

            if (user != null)
            {
                _logger.LogInformation("User logged in: " + user);
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
                _logger.LogInformation("Generating JWT token: " + userName);
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
                _logger.LogInformation("Claims added: " + claims);
                var tokenOptions = new JwtSecurityToken(
                    issuer: configIssuer,
                    audience: configAudience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30), // Adjust the token expiration as needed
                    signingCredentials: signinCredentials
                );

                _logger.LogInformation("Token generated: " + tokenOptions);
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Token not generated: " + ex);
                throw new Exception("An error occurred while generating a JWT token.", ex);
            }
        }

        private bool IsValidEmail(string email)
        {
            // Use a regular expression to check if the email is in a valid format
            // This is a basic example; you can use more comprehensive patterns for email validation
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            _logger.LogInformation("Validating email: " + email);
            return Regex.IsMatch(email, emailPattern);
        }

        private bool SendEmailVerification(string email, string registrationToken)
        {
            try
            {
                _logger.LogInformation("Sending email verification: " + email);
                var emailVerificationPath = _config.GetSection("Authentication").GetSection("Swagger").GetSection("EmailVerificationPath").Value;

                // Construct the email content with a link containing the registration token.
                var emailContent = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
        }}
        .header {{
            background-color: #007BFF;
            color: #ffffff;
            padding: 20px;
        }}
        .content {{
            padding: 20px;
        }}
        .button {{
            display: inline-block;
            background-color: #007BFF;
            color: #ffffff;
            padding: 10px 20px;
            text-decoration: none;
            border-radius: 5px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Thank You for Registering!</h1>
        </div>
        <div class='content'>
            <p>Dear User,</p>
            <p>Thank you for registering with our .NET Core API. To activate your account, please click the button below:</p>
            <a class='button' href='{emailVerificationPath}{registrationToken}'>Verify Your Email</a>
            <p>This link will expire in the next 24 hours.</p>
            <p>If you have any questions or need assistance, please feel free to contact our support team by replying to this email.</p>
            <p>Welcome to my community!</p>
        </div>
    </div>
</body>
</html>
";

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
