using AutoMapper;
using Moq;
using npascu_api_v1.Models.DTOs.Order;
using npascu_api_v1.Models.Entities;
using npascu_api_v1.Repository.Interface;
using npascu_api_v1.Services.Implementation;
using NUnit.Framework;

namespace npascu_api_v1_tests.ServiceTests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private OrderService _orderService;
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void Setup()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _mapperMock = new Mock<IMapper>();
            _orderService = new OrderService(_orderRepositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public void GetOrders_ReturnsOrders()
        {
            // Arrange
            var orders = new List<Order> { new Order { Id = 1 } };
            var orderDtos = new List<OrderDto> { new OrderDto { Id = 1 } };

            _orderRepositoryMock.Setup(repo => repo.GetOrders()).Returns(orders);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<Order>, IEnumerable<OrderDto>>(orders)).Returns(orderDtos);

            // Act
            var result = _orderService.GetOrders();

            // Assert
            CollectionAssert.AreEqual(orderDtos, result);
        }

        [Test]
        public void CreateOrder_ReturnsCreatedOrder()
        {
            // Arrange
            var orderDto = new OrderDto { Id = 1 };
            var order = new Order { Id = 1 };
            var createOrderDto = new CreateOrderDto {  };

            _mapperMock.Setup(mapper => mapper.Map<Order>(createOrderDto)).Returns(order);
            _orderRepositoryMock.Setup(repo => repo.CreateOrder(order)).Returns(order);
            _mapperMock.Setup(mapper => mapper.Map<OrderDto>(order)).Returns(orderDto);

            // Act
            var result = _orderService.CreateOrder(createOrderDto);

            // Assert
            Assert.AreEqual(orderDto.Id, result.Id);
        }

        [Test]
        public void UpdateOrder_ReturnsUpdatedOrder()
        {
            // Arrange
            int orderId = 1;
            var orderDto = new OrderDto { Id = orderId };
            var updatedOrder = new Order { Id = orderId };

            _mapperMock.Setup(mapper => mapper.Map<Order>(orderDto)).Returns(updatedOrder);
            _orderRepositoryMock.Setup(repo => repo.UpdateOrder(orderId, updatedOrder)).Returns(updatedOrder);
            _mapperMock.Setup(mapper => mapper.Map<OrderDto>(updatedOrder)).Returns(orderDto);

            // Act
            var result = _orderService.UpdateOrder(orderId, orderDto);

            // Assert
            Assert.AreEqual(orderDto, result);
        }

        [Test]
        public void UpdateOrder_ReturnsNullWhenNotFound()
        {
            // Arrange
            int orderId = 1;
            var orderDto = new OrderDto { Id = orderId };

            _orderRepositoryMock.Setup(repo => repo.UpdateOrder(orderId, It.IsAny<Order>())).Returns((Order)null);

            // Act
            var result = _orderService.UpdateOrder(orderId, orderDto);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void DeleteOrder_ReturnsTrue()
        {
            // Arrange
            int orderId = 1;
            _orderRepositoryMock.Setup(repo => repo.DeleteOrder(orderId)).Returns(true);

            // Act
            var result = _orderService.DeleteOrder(orderId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteOrder_ReturnsFalse()
        {
            // Arrange
            int orderId = 1;
            _orderRepositoryMock.Setup(repo => repo.DeleteOrder(orderId)).Returns(false);

            // Act
            var result = _orderService.DeleteOrder(orderId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void GetOrderById_ReturnsOrder()
        {
            // Arrange
            int orderId = 1;
            var order = new Order { Id = orderId };
            var orderDto = new OrderDto { Id = orderId };

            _orderRepositoryMock.Setup(repo => repo.GetOrderById(orderId)).Returns(order);
            _mapperMock.Setup(mapper => mapper.Map<OrderDto>(order)).Returns(orderDto);

            // Act
            var result = _orderService.GetOrderById(orderId);

            // Assert
            Assert.AreEqual(orderDto, result);
        }

        [Test]
        public void GetOrderById_ReturnsNullWhenNotFound()
        {
            // Arrange
            int orderId = 1;

            _orderRepositoryMock.Setup(repo => repo.GetOrderById(orderId)).Returns((Order)null);

            // Act
            var result = _orderService.GetOrderById(orderId);

            // Assert
            Assert.IsNull(result);
        }
    }
}