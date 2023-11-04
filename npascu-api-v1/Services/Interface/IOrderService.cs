using npascu_api_v1.Models.DTOs;

namespace npascu_api_v1.Services.Interface
{
    public interface IOrderService
    {
        IEnumerable<OrderDto> GetOrders();
        OrderDto CreateOrder(OrderDto orderDto);
        OrderDto UpdateOrder(int orderId, OrderDto orderDto);
        bool DeleteOrder(int orderId);
        OrderDto GetOrderById(int orderId);
    }
}
