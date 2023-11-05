using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using npascu_api_v1.Controllers;
using npascu_api_v1.Models.DTOs.User;
using npascu_api_v1.Services.Interface;
using NUnit.Framework;

namespace npascu_api_v1_tests.ControllerTests
{
    [TestFixture]
    public class UserControllerTests
    {
        private UserController _controller;
        private Mock<ILogger<UserController>> _loggerMock;
        private Mock<IUserService> _userServiceMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<UserController>>();
            _userServiceMock = new Mock<IUserService>();
            _controller = new UserController(_loggerMock.Object, _userServiceMock.Object);
        }

        [Test]
        public void GetUsers_ReturnsEmptyList()
        {
            // Arrange
            _userServiceMock.Setup(service => service.GetUsers()).Returns(new List<UserDto>());

            // Act
            var result = _controller.GetUsers();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult, Is.Not.Null);
        }

        [Test]
        public void GetUsers_ReturnsUsers()
        {
            // Arrange
            var expectedUsers = new List<UserDto> { new UserDto { Id = 1, FirstName = "User1" } };
            _userServiceMock.Setup(service => service.GetUsers()).Returns(expectedUsers);

            // Act
            var result = _controller.GetUsers();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.AreEqual(expectedUsers, (IEnumerable<UserDto>)okResult.Value);
        }

        [Test]
        public void CreateUser_ReturnsCreated()
        {
            var newUser = new CreateUserDto { FirstName = "NewUser" };
            var createdUser = new UserDto { FirstName = "NewUser" };
            _userServiceMock.Setup(service => service.CreateUser(newUser)).Returns(createdUser);

            var result = _controller.CreateUser(newUser);

            Assert.IsNotNull(result.Result);
            var okResult = (OkObjectResult)result.Result;
            var user = (UserDto)okResult.Value;

            Assert.AreEqual(createdUser.FirstName, user.FirstName);
        }

        [Test]
        public void GetUser_ReturnsUser()
        {
            int userId = 1;
            var user = new UserDto { Id = userId, FirstName = "User1" };
            _userServiceMock.Setup(service => service.GetUserById(userId)).Returns(user);

            var result = _controller.GetUser(userId);

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.IsAssignableFrom<UserDto>(okResult.Value);
            Assert.AreEqual(user, (UserDto)okResult.Value);
        }

        [Test]
        public void GetUser_ReturnsNotFound()
        {
            int userId = 1;
            _userServiceMock.Setup(service => service.GetUserById(userId)).Returns((UserDto)null);

            var result = _controller.GetUser(userId);

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public void UpdateUser_ReturnsUpdatedUser()
        {
            int userId = 1;
            var userDto = new UserDto { Id = userId, FirstName = "UpdatedUser" };
            _userServiceMock.Setup(service => service.UpdateUser(userId, userDto)).Returns(userDto);

            var result = _controller.UpdateUser(userId, userDto);

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.IsAssignableFrom<UserDto>(okResult.Value);
            Assert.AreEqual(userDto, (UserDto)okResult.Value);
        }

        [Test]
        public void UpdateUser_ReturnsNotFound()
        {
            int userId = 1;
            var userDto = new UserDto { Id = userId, FirstName = "UpdatedUser" };
            _userServiceMock.Setup(service => service.UpdateUser(userId, userDto)).Returns((UserDto)null);

            var result = _controller.UpdateUser(userId, userDto);

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public void DeleteUser_ReturnsNoContent()
        {
            int userId = 1;
            _userServiceMock.Setup(service => service.DeleteUser(userId)).Returns(true);

            var result = _controller.DeleteUser(userId);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public void DeleteUser_ReturnsNotFound()
        {
            int userId = 1;
            _userServiceMock.Setup(service => service.DeleteUser(userId)).Returns(false);

            var result = _controller.DeleteUser(userId);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        // Add more test methods for other UserController actions, including error cases
    }
}
