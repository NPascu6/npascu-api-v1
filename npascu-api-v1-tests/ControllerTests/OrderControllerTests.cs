using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using npascu_api_v1.Controllers;
using npascu_api_v1.Models.DTOs;
using npascu_api_v1.Services.Interface;
using NUnit.Framework;

namespace npascu_api_v1_tests.ControllerTests
{
    [TestFixture]
    public class OrderControllerTests
    {
        private OrderController _controller;
        private Mock<ILogger<OrderController>> _loggerMock;
        private Mock<IOrderService> _orderServiceMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<OrderController>>();
            _orderServiceMock = new Mock<IOrderService>();
            _controller = new OrderController(_loggerMock.Object, _orderServiceMock.Object);
        }

        [Test]
        public void GetOrders_ReturnsEmptyList()
        {
            _orderServiceMock.Setup(service => service.GetOrders()).Returns(new List<OrderDto>());

            var result = _controller.GetOrders();

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.IsEmpty((IEnumerable<OrderDto>)okResult.Value);
        }

        [Test]
        public void GetOrders_ReturnsOrders()
        {
            var expectedOrders = new List<OrderDto> { new OrderDto { Id = 1 } };
            _orderServiceMock.Setup(service => service.GetOrders()).Returns(expectedOrders);

            var result = _controller.GetOrders();

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.AreEqual(expectedOrders, (IEnumerable<OrderDto>)okResult.Value);
        }

        [Test]
        public void CreateOrder_ReturnsCreated()
        {
            var newOrder = new OrderDto {  };
            var createdOrder = new OrderDto { Id = 1 };
            _orderServiceMock.Setup(service => service.CreateOrder(newOrder)).Returns(createdOrder);

            var result = _controller.CreateOrder(newOrder);

            var okResult = (OkObjectResult)result.Result;
            Assert.IsAssignableFrom<OrderDto>(okResult.Value);
            Assert.AreEqual(createdOrder, (OrderDto)okResult.Value);
        }

        [Test]
        public void UpdateOrder_ReturnsUpdatedOrder()
        {
            int orderId = 1;
            var orderDto = new OrderDto { Id = orderId };
            _orderServiceMock.Setup(service => service.UpdateOrder(orderId, orderDto)).Returns(orderDto);

            var result = _controller.UpdateOrder(orderId, orderDto);

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.IsAssignableFrom<OrderDto>(okResult.Value);
            Assert.AreEqual(orderDto, (OrderDto)okResult.Value);
        }

        [Test]
        public void UpdateOrder_ReturnsNotFound()
        {
            int orderId = 1;    
            var orderDto = new OrderDto { Id = orderId };
            _orderServiceMock.Setup(service => service.UpdateOrder(orderId, orderDto)).Returns((OrderDto)null);

            var result = _controller.UpdateOrder(orderId, orderDto);

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public void DeleteOrder_ReturnsNoContent()
        {
            int orderId = 1;
            _orderServiceMock.Setup(service => service.DeleteOrder(orderId)).Returns(true);

            var result = _controller.DeleteOrder(orderId);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public void DeleteOrder_ReturnsNotFound()
        {
            int orderId = 1;
            _orderServiceMock.Setup(service => service.DeleteOrder(orderId)).Returns(false);

            var result = _controller.DeleteOrder(orderId);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        // Add more test methods for other OrderController actions, including error cases
    }
}
