using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using npascu_api_v1.Models.DTOs.Item;
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
        /// Get all items
        /// </summary>
        /// <returns>A list of Items</returns>
        [Authorize]
        [HttpGet("GetItems")]
        public ActionResult<IEnumerable<ItemDto>> GetItems()
        {
            try
            {
                _logger.LogInformation("GetItems called");
                var items = _itemService.GetItems();

                if (items == null || !items.Any())
                {
                    return Ok(new List<ItemDto>()); // HTTP 200 Epty item list.
                }

                return Ok(items); // HTTP 200 - OK
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.", ex.Message);
                return StatusCode(500, "Internal Server Error" + ex.Message); // HTTP 500 - Internal Server Error
            }
        }

        [Authorize]
        [HttpPost("CreateItem")]
        public ActionResult<ItemDto> CreateItem([FromBody] ItemDto itemDto)
        {
            try
            {
                // Perform validation and item creation logic here
                var createdItem = _itemService.CreateItem(itemDto);

                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an item.");
                return StatusCode(500, "Internal Server Error"); // HTTP 500 - Internal Server Error
            }
        }

        [Authorize]
        [HttpPut("UpdateItem/{id}")]
        public ActionResult<ItemDto> UpdateItem(int id, [FromBody] ItemDto itemDto)
        {
            try
            {
                // Perform validation and item update logic here
                var updatedItem = _itemService.UpdateItem(id, itemDto);

                if (updatedItem == null)
                {
                    return NotFound(); // HTTP 404 - Not Found
                }

                return Ok(updatedItem); // HTTP 200 - OK
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating an item.");
                return StatusCode(500, "Internal Server Error"); // HTTP 500 - Internal Server Error
            }
        }

        [Authorize]
        [HttpDelete("DeleteItem/{id}")]
        public ActionResult DeleteItem(int id)
        {
            try
            {
                var isDeleted = _itemService.DeleteItem(id);

                if (!isDeleted)
                {
                    return NotFound(); // HTTP 404 - Not Found
                }

                return NoContent(); // HTTP 204 - No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting an item.");
                return StatusCode(500, "Internal Server Error"); // HTTP 500 - Internal Server Error
            }
        }
    }
}
