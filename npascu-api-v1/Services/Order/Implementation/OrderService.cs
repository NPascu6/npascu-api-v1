using AutoMapper;
using npascu_api_v1.Models.DTOs.Order;
using npascu_api_v1.Models.Entities;
using npascu_api_v1.Repository.Interface;
using npascu_api_v1.Services.Interface;

namespace npascu_api_v1.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public IEnumerable<OrderDto> GetOrders()
        {
            try
            {
                var orders = _orderRepository.GetOrders();
                return _mapper.Map<IEnumerable<Order>, IEnumerable<OrderDto>>(orders);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting orders.", ex);
            }
        }

        public OrderDto CreateOrder(CreateOrderDto orderDto)
        {
            try
            {
                var order = _mapper.Map<Order>(orderDto);
                // Perform any necessary validation or business logic here
                var createdOrder = _orderRepository.CreateOrder(order);
                return _mapper.Map<OrderDto>(createdOrder);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating an order.", ex);
            }
        }

        public OrderDto UpdateOrder(int orderId, OrderDto orderDto)
        {
            try
            {
                var updatedOrder = _mapper.Map<Order>(orderDto);
                // Perform any necessary validation or business logic here
                var existingOrder = _orderRepository.UpdateOrder(orderId, updatedOrder);

                if (existingOrder == null)
                {
                    return null; // Order with the specified ID not found
                }

                return _mapper.Map<OrderDto>(existingOrder);
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
                return _orderRepository.DeleteOrder(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting an order.", ex);
            }
        }

        public OrderDto GetOrderById(int orderId)
        {
            try
            {
                var order = _orderRepository.GetOrderById(orderId);

                if (order == null)
                {
                    return null; // Order with the specified ID not found
                }

                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting an order by ID.", ex);
            }
        }
    }
}
