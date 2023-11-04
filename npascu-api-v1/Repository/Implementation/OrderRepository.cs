using Microsoft.EntityFrameworkCore;
using npascu_api_v1.Models.Entities;
using npascu_api_v1.Repository.Interface;

namespace npascu_api_v1.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Order> GetOrders()
        {
            try
            {
                var orders = _context.Orders
                    .Include(o => o.User) // Include related entities as needed
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                    .ToList();
                return orders;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting orders.", ex);
            }
        }

        public Order CreateOrder(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                _context.SaveChanges();
                return order;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating an order.", ex);
            }
        }

        public Order UpdateOrder(int orderId, Order updatedOrder)
        {
            try
            {
                var existingOrder = _context.Orders.Find(orderId);

                if (existingOrder == null)
                {
                    return null; // Order with the specified ID not found
                }

                _context.Entry(existingOrder).CurrentValues.SetValues(updatedOrder);
                _context.SaveChanges();
                return existingOrder;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating an order.", ex);
            }
        }

        public bool DeleteOrder(int orderId)
        {
            try
            {
                var orderToDelete = _context.Orders.Find(orderId);

                if (orderToDelete == null)
                {
                    return false; // Order with the specified ID not found
                }

                _context.Orders.Remove(orderToDelete);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting an order.", ex);
            }
        }

        public Order GetOrderById(int orderId)
        {
            try
            {
                var order = _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                    .FirstOrDefault(o => o.Id == orderId);
                return order;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting an order by ID.", ex);
            }
        }
    }
}
