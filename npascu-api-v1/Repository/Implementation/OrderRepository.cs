using npascu_api_v1.Models.Entities;
using npascu_api_v1.Repository.Interface;

namespace npascu_api_v1.Repository.Implementation
{
    public class OrderRepository: IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Order> GetOrders()
        {
            return _context.Orders.ToList();
        }
    }
}
