using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using npascu_api_v1.Common.Utils;
using npascu_api_v1.Data;
using npascu_api_v1.Modules.DTOs;
using npascu_api_v1.Modules.Services;

namespace npascu_api_v1.Modules.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await context.Users
                .Include(u => u.UserRoles)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    AuthProvider = u.PasswordHash != null ? "Local" : "Google",
                    UserRoles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                })
                .ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound($"User with id {id} not found.");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                AuthProvider = user.PasswordHash != null ? "Local" : "Google"
            };
            return Ok(userDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound($"User with id {id} not found.");
            }

            user.Name = request.Name;
            user.Email = request.Email;

            if (user.PasswordHash != null && !string.IsNullOrEmpty(request.Password))
            {
                user.PasswordHash = PasswordHelper.HashPassword(request.Password);
            }

            context.Entry(user).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound($"User with id {id} not found.");
            }

            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}