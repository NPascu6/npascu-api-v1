using Microsoft.AspNetCore.Mvc;
using npascu_api_v1.Models.DTOs;
using npascu_api_v1.Services.Interface;

namespace npascu_api_v1.Controllers
{
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

        [HttpGet]
        [Route("GetUsers")]
        public IEnumerable<UserDto> Get()
        {
            _logger.LogInformation("GetUsers called");
            return _userService.GetUsers();
        }
    }
}