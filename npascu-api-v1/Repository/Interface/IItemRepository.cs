using npascu_api_v1.Models.Entities;

namespace npascu_api_v1.Repository.Interface
{
    public interface IItemRepository
    {
        IEnumerable<Item> GetItems();
        Item CreateItem(Item item);
        Item UpdateItem(int itemId, Item updatedItem);
        bool DeleteItem(int itemId);
        Item GetItemById(int itemId);

    }
}
