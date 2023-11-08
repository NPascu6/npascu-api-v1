using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using npascu_api_v1.Models.DTOs.Auth;
using npascu_api_v1.Repository.Interface;
using npascu_api_v1.Services.Email.Interface;
using npascu_api_v1.Services.Implementation;
using NUnit.Framework;

namespace npascu_api_v1_tests.ServiceTests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private AuthService authService;
        private Mock<IAuthRepository> authRepository;
        private Mock<IConfiguration> configuration;
        private Mock<IEmailService> emailService;
        private ILogger<AuthService> _logger;

        [SetUp]
        public void Setup()
        {
            authRepository = new Mock<IAuthRepository>();
            configuration = new Mock<IConfiguration>();
            emailService = new Mock<IEmailService>();
            authService = new AuthService(configuration.Object, authRepository.Object, emailService.Object, _logger);
        }


        [Test]
        public void Login_InvalidUser_ReturnsNull()
        {
            // Arrange
            string username = "invalidUser";
            string password = "invalidPassword";

            authRepository.Setup(repo => repo.LoginUserAsync(username, password)).Returns((LoginModel)null); // Replace User with your actual user object.

            // Act
            string token = authService.Login(username, password);

            // Assert
            Assert.IsNull(token);
        }

        [Test]
        public void Register_InvalidEmail_ReturnsNull()
        {
            // Arrange
            string username = "newUser";
            string email = "invalid-email"; // Invalid email address
            string password = "newPassword";

            // Act
            string token = authService.Register(username, email, password);

            // Assert
            Assert.IsNull(token);
        }

        [Test]
        public void Register_UserRegistrationFailed_ReturnsNull()
        {
            // Arrange
            string username = "newUser";
            string email = "valid@example.com";
            string password = "newPassword";

            authRepository.Setup(repo => repo.RegisterUserAsync(username, email, password)).Returns((RegisterModel)null); // Replace User with your actual user object.

            // Act
            string token = authService.Register(username, email, password);

            // Assert
            Assert.IsNull(token);
        }
    }
}
