using Microsoft.AspNetCore.Mvc;
using npascu_api_v1.Models.DTOs;
using npascu_api_v1.Services.Interface;

namespace npascu_api_v1.Controllers
{
    /// <summary>
    /// Get's all the orders
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        [HttpGet]
        [Route("GetOrders")]
        public IEnumerable<OrderDto> GetOrders()
        {
            _logger.LogInformation("GetOrders called");
            return _orderService.GetOrders();
        }
    }
}
