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

        public Item CreateItem(Item item)
        {
            try
            {
                _context.Items.Add(item);
                _context.SaveChanges();
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating an item.", ex);
            }
        }

        public Item UpdateItem(int itemId, Item updatedItem)
        {
            try
            {
                var existingItem = _context.Items.Find(itemId);

                if (existingItem == null)
                {
                    return null; // Item with the specified ID not found
                }

                _context.Entry(existingItem).CurrentValues.SetValues(updatedItem);
                _context.SaveChanges();
                return existingItem;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating an item.", ex);
            }
        }

        public bool DeleteItem(int itemId)
        {
            try
            {
                var itemToDelete = _context.Items.Find(itemId);

                if (itemToDelete == null)
                {
                    return false; // Item with the specified ID not found
                }

                _context.Items.Remove(itemToDelete);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting an item.", ex);
            }
        }

        public Item GetItemById(int itemId)
        {
            try
            {
                var item = _context.Items.Find(itemId);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting an item by ID.", ex);
            }
        }


    }
}
