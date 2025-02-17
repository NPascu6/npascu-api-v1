using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using npascu_api_v1.Data;
using npascu_api_v1.Data.Models;

namespace npascu_api_v1.Modules.Users;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await context.Users.ToListAsync();
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

        return user ?? throw new InvalidOperationException($"User Not Found.");
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<bool> PutUser(int id, User user)
    {
        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (existingUser == null)
        {
            return false;
        }

        existingUser.Name = user.Name;
        existingUser.Email = user.Email;
        existingUser.PasswordHash = user.PasswordHash;

        context.Entry(existingUser).State = EntityState.Modified;

        await context.SaveChangesAsync();

        return true;
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<bool> DeleteUser(int id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return false;
        }

        context.Users.Remove(user);

        await context.SaveChangesAsync();

        return true;
    }
}