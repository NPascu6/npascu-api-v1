using Microsoft.AspNetCore.Mvc;
using npascu_api_v1.Models.DTOs.User;
using npascu_api_v1.Services.Interface;

namespace npascu_api_v1.Controllers
{
    /// <summary>
    /// Get's all the users
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet("GetUsers")]
        public ActionResult<IEnumerable<UserDto>> GetUsers()
        {
            try
            {
                _logger.LogInformation("GetUsers called");
                var users = _userService.GetUsers();

                if (users == null || !users.Any())
                {
                    return Ok(new List<UserDto>()); // HTTP 200 - OK with an empty array
                }

                return Ok(users); // HTTP 200 - OK
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, "Internal Server Error"); // HTTP 500 - Internal Server Error
            }
        }

        [HttpPost("CreateUser")]
        public ActionResult<UserDto> CreateUser([FromBody] CreateUserDto userDto)
        {
            try
            {
                // Perform validation and user creation logic here
                var createdUser = _userService.CreateUser(userDto);

                return Ok(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a user.");
                return StatusCode(500, "Internal Server Error"); // HTTP 500 - Internal Server Error
            }
        }

        [HttpGet("GetUser/{id}")]
        public ActionResult<UserDto> GetUser(int id)
        {
            try
            {
                var user = _userService.GetUserById(id);

                if (user == null)
                {
                    return NotFound(); // HTTP 404 - Not Found
                }

                return Ok(user); // HTTP 200 - OK
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting a user.");
                return StatusCode(500, "Internal Server Error"); // HTTP 500 - Internal Server Error
            }
        }

        [HttpPut("UpdateUser/{id}")]
        public ActionResult<UserDto> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            try
            {
                // Perform validation and user update logic here
                var updatedUser = _userService.UpdateUser(id, userDto);

                if (updatedUser == null)
                {
                    return NotFound(); // HTTP 404 - Not Found
                }

                return Ok(updatedUser); // HTTP 200 - OK
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating a user.");
                return StatusCode(500, "Internal Server Error"); // HTTP 500 - Internal Server Error
            }
        }

        [HttpDelete("DeleteUser/{id}")]
        public ActionResult DeleteUser(int id)
        {
            try
            {
                var isDeleted = _userService.DeleteUser(id);

                if (!isDeleted)
                {
                    return NotFound(); // HTTP 404 - Not Found
                }

                return NoContent(); // HTTP 204 - No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting a user.");
                return StatusCode(500, "Internal Server Error"); // HTTP 500 - Internal Server Error
            }
        }
    }
}