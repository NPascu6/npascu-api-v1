using Microsoft.EntityFrameworkCore;
using npascu_api_v1.Models.Entities;
using npascu_api_v1.Repository.Implementation;
using npascu_api_v1.Repository;
using NUnit.Framework;

namespace npascu_api_v1_tests.RepositoryTests
{
    public class OrderRepositoryTests
    {
        private DbContextOptions<AppDbContext> _options;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderRepositoryTests")
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
        public void GetOrders_ShouldReturnAllOrders()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new OrderRepository(context);
                repository.CreateOrder(new Order { Id = 1, OrderItems = new List<OrderItem>(), User = new User(), TotalPrice = 100.0m });
                repository.CreateOrder(new Order { Id = 2, OrderItems = new List<OrderItem>(), User = new User(), TotalPrice = 200.0m });

                // Act
                var orders = repository.GetOrders();

                // Assert
                Assert.AreEqual(2, orders.Count());
            }
        }

        [Test]
        public void CreateOrder_ShouldCreateNewOrder()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new OrderRepository(context);
                var newOrder = new Order { User = new User(), TotalPrice = 150.0m };

                // Act
                var createdOrder = repository.CreateOrder(newOrder);

                // Assert
                Assert.IsNotNull(createdOrder);
                Assert.AreEqual(newOrder.TotalPrice, createdOrder.TotalPrice);
            }
        }

        [Test]
        public void UpdateOrder_ShouldUpdateExistingOrder()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new OrderRepository(context);
                repository.CreateOrder(new Order() { Id = 1 });
                int orderId = 1;
                var updatedOrder = new Order { Id = orderId, TotalPrice = 300.0m };

                // Act
                var result = repository.UpdateOrder(orderId, updatedOrder);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(updatedOrder.TotalPrice, result.TotalPrice);
            }
        }

        [Test]
        public void UpdateOrder_ShouldReturnNullForNonExistentOrder()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new OrderRepository(context);
                int orderId = 999;
                var updatedOrder = new Order { Id = orderId };

                // Act
                var result = repository.UpdateOrder(orderId, updatedOrder);

                // Assert
                Assert.IsNull(result);
            }
        }

        [Test]
        public void DeleteOrder_ShouldDeleteExistingOrder()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new OrderRepository(context);
                repository.CreateOrder(new Order { Id = 1 });
                int orderId = 1;

                // Act
                var result = repository.DeleteOrder(orderId);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void DeleteOrder_ShouldReturnFalseForNonExistentOrder()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new OrderRepository(context);
                int orderId = 999;

                // Act
                var result = repository.DeleteOrder(orderId);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void GetOrderById_ShouldReturnExistingOrder()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new OrderRepository(context);
                int orderId = 1;
                context.Orders.Add(new Order { Id = orderId, OrderItems = new List<OrderItem>(), User = new User(), TotalPrice = 100.0m });
                context.SaveChanges();

                // Act
                var order = repository.GetOrderById(orderId);

                // Assert
                Assert.IsNotNull(order);
                Assert.AreEqual(orderId, order.Id);
            }
        }

        [Test]
        public void GetOrderById_ShouldReturnNullForNonExistentOrder()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new OrderRepository(context);
                int orderId = 999;

                // Act
                var order = repository.GetOrderById(orderId);

                // Assert
                Assert.IsNull(order);
            }
        }
    }
}
