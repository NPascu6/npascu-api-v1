using AutoMapper;
using Moq;
using npascu_api_v1.Models.DTOs;
using npascu_api_v1.Models.Entities;
using npascu_api_v1.Repository.Interface;
using npascu_api_v1.Services.Implementation;
using NUnit.Framework;

namespace npascu_api_v1_tests.ServiceTests
{
    [TestFixture]
    public class ItemServiceTests
    {
        private ItemService _itemService;
        private Mock<IItemRepository> _itemRepositoryMock;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void Setup()
        {
            _itemRepositoryMock = new Mock<IItemRepository>();
            _mapperMock = new Mock<IMapper>();
            _itemService = new ItemService(_itemRepositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public void GetItems_ReturnsItems()
        {
            // Arrange
            var items = new List<Item> { new Item { Id = 1, Name = "Item1" } };
            var itemDtos = new List<ItemDto> { new ItemDto { Id = 1, Name = "Item1" } };

            _itemRepositoryMock.Setup(repo => repo.GetItems()).Returns(items);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<ItemDto>>(items)).Returns(itemDtos);

            // Act
            var result = _itemService.GetItems();

            // Assert
            CollectionAssert.AreEqual(itemDtos, result);
        }

        [Test]
        public void GetItems_ReturnsEmptyList()
        {
            // Arrange
            _itemRepositoryMock.Setup(repo => repo.GetItems()).Returns((IEnumerable<Item>)null);

            // Act
            var result = _itemService.GetItems();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void CreateItem_ReturnsCreatedItem()
        {
            // Arrange
            var itemDto = new ItemDto { Name = "NewItem" };
            var item = new Item { Id = 1, Name = "NewItem" };

            _mapperMock.Setup(mapper => mapper.Map<Item>(itemDto)).Returns(item);
            _itemRepositoryMock.Setup(repo => repo.CreateItem(item)).Returns(item);
            _mapperMock.Setup(mapper => mapper.Map<ItemDto>(item)).Returns(itemDto);

            // Act
            var result = _itemService.CreateItem(itemDto);

            // Assert
            Assert.AreEqual(itemDto, result);
        }

        [Test]
        public void UpdateItem_ReturnsUpdatedItem()
        {
            // Arrange
            int itemId = 1;
            var itemDto = new ItemDto { Id = itemId, Name = "UpdatedItem" };
            var updatedItem = new Item { Id = itemId, Name = "UpdatedItem" };

            _mapperMock.Setup(mapper => mapper.Map<Item>(itemDto)).Returns(updatedItem);
            _itemRepositoryMock.Setup(repo => repo.UpdateItem(itemId, updatedItem)).Returns(updatedItem);
            _mapperMock.Setup(mapper => mapper.Map<ItemDto>(updatedItem)).Returns(itemDto);

            // Act
            var result = _itemService.UpdateItem(itemId, itemDto);

            // Assert
            Assert.AreEqual(itemDto, result);
        }

        [Test]
        public void UpdateItem_ReturnsNullWhenNotFound()
        {
            // Arrange
            int itemId = 1;
            var itemDto = new ItemDto { Id = itemId, Name = "UpdatedItem" };

            _itemRepositoryMock.Setup(repo => repo.UpdateItem(itemId, It.IsAny<Item>())).Returns((Item)null);

            // Act
            var result = _itemService.UpdateItem(itemId, itemDto);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void DeleteItem_ReturnsTrue()
        {
            // Arrange
            int itemId = 1;
            _itemRepositoryMock.Setup(repo => repo.DeleteItem(itemId)).Returns(true);

            // Act
            var result = _itemService.DeleteItem(itemId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteItem_ReturnsFalse()
        {
            // Arrange
            int itemId = 1;
            _itemRepositoryMock.Setup(repo => repo.DeleteItem(itemId)).Returns(false);

            // Act
            var result = _itemService.DeleteItem(itemId);

            // Assert
            Assert.IsFalse(result);
        }

        // Add more test methods to cover additional scenarios and error cases
    }
}
