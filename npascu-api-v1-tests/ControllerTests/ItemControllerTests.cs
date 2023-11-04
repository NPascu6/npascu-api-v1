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
    public class ItemControllerTests
    {
        private ItemController _controller;
        private Mock<ILogger<ItemController>> _loggerMock;
        private Mock<IItemService> _itemServiceMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<ItemController>>();
            _itemServiceMock = new Mock<IItemService>();
            _controller = new ItemController(_loggerMock.Object, _itemServiceMock.Object);
        }

        [Test]
        public void GetItems_ReturnsEmptyList()
        {
            _itemServiceMock.Setup(service => service.GetItems()).Returns(new List<ItemDto>());

            var result = _controller.GetItems();

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.IsEmpty((IEnumerable<ItemDto>)okResult.Value);
        }

        [Test]
        public void GetItems_ReturnsItems()
        {
            var expectedItems = new List<ItemDto> { new ItemDto { Id = 1, Name = "Item1" } };
            _itemServiceMock.Setup(service => service.GetItems()).Returns(expectedItems);

            var result = _controller.GetItems();

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.AreEqual(expectedItems, (IEnumerable<ItemDto>)okResult.Value);
        }

        [Test]
        public void CreateItem_ReturnsCreated()
        {
            var newItem = new ItemDto { Name = "NewItem" };
            var createdItem = new ItemDto { Id = 1, Name = "NewItem" };
            _itemServiceMock.Setup(service => service.CreateItem(newItem)).Returns(createdItem);

            var result = _controller.CreateItem(newItem);

            var okResult = (OkObjectResult)result.Result;
            Assert.IsAssignableFrom<ItemDto>(okResult.Value);
            Assert.AreEqual(createdItem, (ItemDto)okResult.Value);
        }

        [Test]
        public void UpdateItem_ReturnsUpdatedItem()
        {
            int itemId = 1;
            var itemDto = new ItemDto { Id = itemId, Name = "UpdatedItem" };
            _itemServiceMock.Setup(service => service.UpdateItem(itemId, itemDto)).Returns(itemDto);

            var result = _controller.UpdateItem(itemId, itemDto);

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.IsAssignableFrom<ItemDto>(okResult.Value);
            Assert.AreEqual(itemDto, (ItemDto)okResult.Value);
        }

        [Test]
        public void UpdateItem_ReturnsNotFound()
        {
            int itemId = 1;
            var itemDto = new ItemDto { Id = itemId, Name = "UpdatedItem" };
            _itemServiceMock.Setup(service => service.UpdateItem(itemId, itemDto)).Returns((ItemDto)null);

            var result = _controller.UpdateItem(itemId, itemDto);

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public void DeleteItem_ReturnsNoContent()
        {
            int itemId = 1;
            _itemServiceMock.Setup(service => service.DeleteItem(itemId)).Returns(true);

            var result = _controller.DeleteItem(itemId);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public void DeleteItem_ReturnsNotFound()
        {
            int itemId = 1;
            _itemServiceMock.Setup(service => service.DeleteItem(itemId)).Returns(false);

            var result = _controller.DeleteItem(itemId);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        // Add more test methods for other ItemController actions, including error cases
    }
}
