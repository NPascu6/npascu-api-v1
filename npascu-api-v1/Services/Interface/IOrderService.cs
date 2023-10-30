using npascu_api_v1.Models.DTOs;

namespace npascu_api_v1.Services.Interface
{
    public interface IOrderService
    {
        IEnumerable<OrderDto> GetOrders();
    }
}
