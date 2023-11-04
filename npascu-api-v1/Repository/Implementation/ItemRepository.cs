using Microsoft.EntityFrameworkCore;
using npascu_api_v1.Models.Entities;
using npascu_api_v1.Repository.Interface;

namespace npascu_api_v1.Repository.Implementation
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _context;
        public ItemRepository(AppDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Item> GetItems()
        {
            try
            {
                var items = _context.Items;
                return items;

            }
            catch(Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
