using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using npascu_api_v1.Models.DTOs;
using npascu_api_v1.Services.Interface;
using System;
using System.Collections.Generic;

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
        /// Get all items
        /// </summary>
        /// <returns>A list of Items</returns>
        [HttpGet("GetItems")]
        public ActionResult<IEnumerable<ItemDto>> GetItems()
        {
            try
            {
                _logger.LogInformation("GetItems called");
                var items = _itemService.GetItems();

                if (items == null || !items.Any())
                {
                    return NoContent(); // HTTP 204 - No items found
                }

                return Ok(items); // HTTP 200 - OK
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, "Internal Server Error"); // HTTP 500 - Internal Server Error
            }
        }
    }
}
