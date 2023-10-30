using npascu_api_v1.Models.Entities;

namespace npascu_api_v1.Repository.Interface
{
    public interface IItemRepository
    {
        IEnumerable<Item> GetItems();
    }
}
