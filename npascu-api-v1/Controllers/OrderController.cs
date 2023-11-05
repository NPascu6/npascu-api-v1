using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using npascu_api_v1.Models.DTOs.Order;
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

        [HttpGet("GetOrders")]
        public ActionResult<IEnumerable<OrderDto>> GetOrders()
        {
            try
            {
                _logger.LogInformation("GetOrders called");
                var orders = _orderService.GetOrders();

                if (orders == null || !orders.Any())
                {
                    return Ok(new List<OrderDto>()); // HTTP 200 - OK with an empty array
                }

                return Ok(orders); // HTTP 200 - OK
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, "Internal Server Error"); // HTTP 500 - Internal Server Error
            }
        }

        [HttpPost("CreateOrder")]
        public ActionResult<OrderDto> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            try
            {
                // Perform validation and order creation logic here
                var createdOrder = _orderService.CreateOrder(orderDto);

                return Ok(createdOrder); // HTTP 201 - Created
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an order.");
                return StatusCode(500, "Internal Server Error"); // HTTP 500 - Internal Server Error
            }
        }

        [HttpGet("GetOrder/{id}")]
        public ActionResult<OrderDto> GetOrder(int id)
        {
            try
            {
                var order = _orderService.GetOrderById(id);

                if (order == null)
                {
                    return NotFound(); // HTTP 404 - Not Found
                }

                return Ok(order); // HTTP 200 - OK
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting an order.");
                return StatusCode(500, "Internal Server Error"); // HTTP 500 - Internal Server Error
            }
        }

        [HttpPut("UpdateOrder/{id}")]
        public ActionResult<OrderDto> UpdateOrder(int id, [FromBody] OrderDto orderDto)
        {
            try
            {
                // Perform validation and order update logic here
                var updatedOrder = _orderService.UpdateOrder(id, orderDto);

                if (updatedOrder == null)
                {
                    return NotFound(); // HTTP 404 - Not Found
                }

                return Ok(updatedOrder); // HTTP 200 - OK
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating an order.");
                return StatusCode(500, "Internal Server Error"); // HTTP 500 - Internal Server Error
            }
        }

        [HttpDelete("DeleteOrder/{id}")]
        public ActionResult DeleteOrder(int id)
        {
            try
            {
                var isDeleted = _orderService.DeleteOrder(id);

                if (!isDeleted)
                {
                    return NotFound(); // HTTP 404 - Not Found
                }

                return NoContent(); // HTTP 204 - No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting an order.");
                return StatusCode(500, "Internal Server Error"); // HTTP 500 - Internal Server Error
            }
        }
    }
}
