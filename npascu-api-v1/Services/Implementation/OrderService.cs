using AutoMapper;
using npascu_api_v1.Models.DTOs;
using npascu_api_v1.Repository.Interface;
using npascu_api_v1.Services.Interface;

namespace npascu_api_v1.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        public OrderService(IMapper mapper, IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
        }
        public IEnumerable<OrderDto> GetOrders()
        {
            var orders = _orderRepository.GetOrders();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }
    }
}
