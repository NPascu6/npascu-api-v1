using Microsoft.AspNetCore.Mvc;
using npascu_api_v1.Models.DTOs;
using npascu_api_v1.Services.Interface;

namespace npascu_api_v1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private readonly IItemService _itemService;

        public ItemController(ILogger<ItemController> logger, IItemService itemService)
        {
            _logger = logger;
            _itemService = itemService;
        }

        /// <summary>
        /// Get's all the items
        /// </summary>
        /// <returns>A list of Items</returns>
        [HttpGet]
        [Route("GetItems")]
        public IEnumerable<ItemDto> GetItems()
        {
            _logger.LogInformation("GetItems called");
            return _itemService.GetItems();
        }
    }
}
