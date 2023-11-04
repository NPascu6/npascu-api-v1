using Microsoft.EntityFrameworkCore;
using npascu_api_v1.Models.Entities;
using npascu_api_v1.Repository.Implementation;
using npascu_api_v1.Repository;
using NUnit.Framework;

namespace npascu_api_v1_tests.RepositoryTests
{
    public class UserRepositoryTests
    {
        private DbContextOptions<AppDbContext> _options;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "UserRepositoryTests")
                .Options;
        }

        [TearDown]
        public void TearDown()
        {
            using (var context = new AppDbContext(_options))
            {
                context.Database.EnsureCreated();
                context.Database.EnsureDeleted();
            }
        }

        [Test]
        public void GetUsers_ShouldReturnAllUsers()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new UserRepository(context);
                repository.CreateUser(new User { Id = 1, Username = "User1" });
                repository.CreateUser(new User { Id = 2, Username = "User2" });

                // Act
                var users = repository.GetUsers();

                // Assert
                Assert.AreEqual(2, users.Count());
            }
        }

        [Test]
        public void CreateUser_ShouldCreateNewUser()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new UserRepository(context);
                var newUser = new User { Username = "NewUser" };

                // Act
                var createdUser = repository.CreateUser(newUser);

                // Assert
                Assert.IsNotNull(createdUser);
                Assert.AreEqual(newUser.Username, createdUser.Username);
            }
        }

        [Test]
        public void UpdateUser_ShouldUpdateExistingUser()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new UserRepository(context);
                repository.CreateUser(new User() { Id = 1 });
                int userId = 1;
                var updatedUser = new User { Id = userId, Username = "UpdatedUser" };

                // Act
                var result = repository.UpdateUser(userId, updatedUser);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(updatedUser.Username, result.Username);
            }
        }

        [Test]
        public void UpdateUser_ShouldReturnNullForNonExistentUser()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new UserRepository(context);
                int userId = 999;
                var updatedUser = new User { Id = userId, Username = "UpdatedUser" };

                // Act
                var result = repository.UpdateUser(userId, updatedUser);

                // Assert
                Assert.IsNull(result);
            }
        }

        [Test]
        public void DeleteUser_ShouldDeleteExistingUser()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new UserRepository(context);
                repository.CreateUser(new User() { Id = 1 });
                int userId = 1;

                // Act
                var result = repository.DeleteUser(userId);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void DeleteUser_ShouldReturnFalseForNonExistentUser()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new UserRepository(context);
                int userId = 999;

                // Act
                var result = repository.DeleteUser(userId);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void GetUserById_ShouldReturnExistingUser()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new UserRepository(context);
                int userId = 1;
                context.Users.Add(new User { Id = userId, Username = "User1" });
                context.SaveChanges();

                // Act
                var user = repository.GetUserById(userId);

                // Assert
                Assert.IsNotNull(user);
                Assert.AreEqual(userId, user.Id);
            }
        }

        [Test]
        public void GetUserById_ShouldReturnNullForNonExistentUser()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new UserRepository(context);
                int userId = 999;

                // Act
                var user = repository.GetUserById(userId);

                // Assert
                Assert.IsNull(user);
            }
        }
    }
}
