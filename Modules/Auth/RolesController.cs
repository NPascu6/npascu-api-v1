using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using npascu_api_v1.Data;
using npascu_api_v1.Data.Models;
using npascu_api_v1.Modules.DTOs;

namespace npascu_api_v1.Modules.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController(ApplicationDbContext context) : ControllerBase
    {
        /// <summary>
        /// Get all roles in the system.
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await context.Roles.ToListAsync();
            return Ok(roles);
        }

        /// <summary>
        /// Get roles for a specific user.
        /// </summary>
        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetUserRoles(int userId)
        {
            var user = await context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var roles = user.UserRoles.Select(ur => ur.Role.Name);
            return Ok(roles);
        }

        /// <summary>
        /// Add a role to a user.
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddRoleToUser([FromBody] UserRoleDto dto)
        {
            var user = await context.Users
                .Include(u => u.UserRoles)
                .SingleOrDefaultAsync(u => u.Id == dto.UserId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var role = await context.Roles.SingleOrDefaultAsync(r => r.Name.ToLower() == dto.RoleName.ToLower());
            if (role == null)
            {
                role = new Role { Name = dto.RoleName };
                context.Roles.Add(role);
                await context.SaveChangesAsync();
            }

            if (user.UserRoles.Any(ur => ur.RoleId == role.Id))
            {
                return BadRequest("User already has this role.");
            }

            // Add the role to the user.
            var userRole = new UserRole { UserId = user.Id, RoleId = role.Id };
            user.UserRoles.Add(userRole);
            await context.SaveChangesAsync();

            return Ok("Role added to user successfully.");
        }

        /// <summary>
        /// Remove a role from a user.
        /// </summary>
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] UserRoleDto dto)
        {
            var userRole = await context.UserRoles
                .Include(ur => ur.Role)
                .SingleOrDefaultAsync(ur =>
                    ur.UserId == dto.UserId && ur.Role.Name.ToLower() == dto.RoleName.ToLower());

            if (userRole == null)
            {
                return NotFound("Role not found for the user.");
            }

            context.UserRoles.Remove(userRole);
            await context.SaveChangesAsync();
            return Ok("Role removed from user successfully.");
        }
    }
}