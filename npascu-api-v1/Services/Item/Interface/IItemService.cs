using npascu_api_v1.Models.DTOs.Item;

namespace npascu_api_v1.Services.Interface
{
    public interface IItemService
    {
        IEnumerable<ItemDto> GetItems();
        ItemDto CreateItem(ItemDto itemDto);
        ItemDto UpdateItem(int itemId, ItemDto itemDto);
        bool DeleteItem(int itemId);
    }
}
