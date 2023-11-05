using AutoMapper;
using Moq;
using npascu_api_v1.Models.DTOs.User;
using npascu_api_v1.Models.Entities;
using npascu_api_v1.Repository.Interface;
using npascu_api_v1.Services.Implementation;
using NUnit.Framework;

namespace npascu_api_v1_tests.ServiceTests
{
    [TestFixture]
    public class UserServiceTests
    {
        private UserService _userService;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _userService = new UserService(_userRepositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public void GetUsers_ReturnsUsers()
        {
            // Arrange
            var users = new List<User> { new User { Id = 1, FirstName = "User1" } };
            var userDtos = new List<UserDto> { new UserDto { Id = 1, FirstName = "User1" } };

            _userRepositoryMock.Setup(repo => repo.GetUsers()).Returns(users);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

            // Act
            var result = _userService.GetUsers();

            // Assert
            CollectionAssert.AreEqual(userDtos, result);
        }

        [Test]
        public void CreateUser_ReturnsCreatedUser()
        {
            // Arrange
            var createUserDto = new CreateUserDto { FirstName = "NewUser" };
            var user = new User { Id = 1, FirstName = "NewUser" };
            var userDto = new UserDto { Id = 1, FirstName = "NewUser" };

            _mapperMock.Setup(mapper => mapper.Map<User>(createUserDto)).Returns(user);
            _userRepositoryMock.Setup(repo => repo.CreateUser(user)).Returns(user);
            _mapperMock.Setup(mapper => mapper.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = _userService.CreateUser(createUserDto);

            // Assert
            Assert.AreEqual(createUserDto.FirstName, result.FirstName);
        }

        [Test]
        public void UpdateUser_ReturnsUpdatedUser()
        {
            // Arrange
            int userId = 1;
            var userDto = new UserDto { Id = userId, FirstName = "UpdatedUser" };
            var updatedUser = new User { Id = userId, FirstName = "UpdatedUser" };

            _mapperMock.Setup(mapper => mapper.Map<User>(userDto)).Returns(updatedUser);
            _userRepositoryMock.Setup(repo => repo.UpdateUser(userId, updatedUser)).Returns(updatedUser);
            _mapperMock.Setup(mapper => mapper.Map<UserDto>(updatedUser)).Returns(userDto);

            // Act
            var result = _userService.UpdateUser(userId, userDto);

            // Assert
            Assert.AreEqual(userDto, result);
        }

        [Test]
        public void UpdateUser_ReturnsNullWhenNotFound()
        {
            // Arrange
            int userId = 1;
            var userDto = new UserDto { Id = userId, FirstName = "UpdatedUser" };

            _userRepositoryMock.Setup(repo => repo.UpdateUser(userId, It.IsAny<User>())).Returns((User)null);

            // Act
            var result = _userService.UpdateUser(userId, userDto);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void DeleteUser_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            _userRepositoryMock.Setup(repo => repo.DeleteUser(userId)).Returns(true);

            // Act
            var result = _userService.DeleteUser(userId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteUser_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            _userRepositoryMock.Setup(repo => repo.DeleteUser(userId)).Returns(false);

            // Act
            var result = _userService.DeleteUser(userId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void GetUserById_ReturnsUser()
        {
            // Arrange
            int userId = 1;
            var user = new User { Id = userId, FirstName = "User1" };
            var userDto = new UserDto { Id = userId, FirstName = "User1" };

            _userRepositoryMock.Setup(repo => repo.GetUserById(userId)).Returns(user);
            _mapperMock.Setup(mapper => mapper.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = _userService.GetUserById(userId);

            // Assert
            Assert.AreEqual(userDto, result);
        }

        [Test]
        public void GetUserById_ReturnsNullWhenNotFound()
        {
            // Arrange
            int userId = 1;

            _userRepositoryMock.Setup(repo => repo.GetUserById(userId)).Returns((User)null);

            // Act
            var result = _userService.GetUserById(userId);

            // Assert
            Assert.IsNull(result);
        }

        // Add more test methods to cover additional scenarios and error cases
    }
}
