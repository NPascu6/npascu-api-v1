using Microsoft.EntityFrameworkCore;
using npascu_api_v1.Models.Entities;
using npascu_api_v1.Repository.Implementation;
using npascu_api_v1.Repository.Interface;
using npascu_api_v1.Repository;
using NUnit.Framework;
namespace npascu_api_v1_tests.RepositoryTests
{
    public class ItemRepositoryTests
    {
        private DbContextOptions<AppDbContext> _options;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "ItemRepositoryTests")
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
        public void GetItems_ShouldReturnAllItems()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                context.Items.Add(new Item { Id = 1, Name = "Item1" });
                context.Items.Add(new Item { Id = 2, Name = "Item2" });
                context.SaveChanges();

                var repository = new ItemRepository(context);

                // Act
                var items = repository.GetItems();

                // Assert
                Assert.AreEqual(2, items.Count());
            }
        }

        [Test]
        public void CreateItem_ShouldCreateNewItem()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new ItemRepository(context);
                var newItem = new Item { Name = "New Item" };

                // Act
                var createdItem = repository.CreateItem(newItem);

                // Assert
                Assert.IsNotNull(createdItem);
                Assert.AreEqual(newItem.Name, createdItem.Name);
            }
        }

        [Test]
        public void UpdateItem_ShouldUpdateExistingItem()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new ItemRepository(context);
                repository.CreateItem(new Item() { Id = 1 });
                int itemId = 1;
                var updatedItem = new Item { Id = itemId, Name = "Updated Item" };

                // Act
                var result = repository.UpdateItem(itemId, updatedItem);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(updatedItem.Name, result.Name);
            }
        }

        [Test]
        public void UpdateItem_ShouldReturnNullForNonExistentItem()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new ItemRepository(context);
                int itemId = 999;
                var updatedItem = new Item { Id = itemId, Name = "Updated Item" };

                // Act
                var result = repository.UpdateItem(itemId, updatedItem);

                // Assert
                Assert.IsNull(result);
            }
        }

        [Test]
        public void DeleteItem_ShouldDeleteExistingItem()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new ItemRepository(context);
                repository.CreateItem(new Item() { Id = 1 });

                int itemId = 1;

                // Act
                var result = repository.DeleteItem(itemId);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void DeleteItem_ShouldReturnFalseForNonExistentItem()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new ItemRepository(context);
                int itemId = 999;

                // Act
                var result = repository.DeleteItem(itemId);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void GetItemById_ShouldReturnExistingItem()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new ItemRepository(context);
                repository.CreateItem(new Item { Id = 1 });
                int itemId = 1;

                // Act
                var item = repository.GetItemById(itemId);

                // Assert
                Assert.IsNotNull(item);
                Assert.AreEqual(itemId, item.Id);
            }
        }

        [Test]
        public void GetItemById_ShouldReturnNullForNonExistentItem()
        {
            using (var context = new AppDbContext(_options))
            {
                // Arrange
                var repository = new ItemRepository(context);
                int itemId = 999;

                // Act
                var item = repository.GetItemById(itemId);

                // Assert
                Assert.IsNull(item);
            }
        }
    }
}
