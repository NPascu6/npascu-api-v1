using npascu_api_v1.Models.Entities;

namespace npascu_api_v1.Repository.Interface
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetOrders();
        Order CreateOrder(Order order);
        Order UpdateOrder(int orderId, Order updatedOrder);
        bool DeleteOrder(int orderId);
        Order GetOrderById(int orderId);
    }
}
