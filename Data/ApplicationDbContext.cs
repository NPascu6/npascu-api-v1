using Microsoft.EntityFrameworkCore;
using npascu_api_v1.Data.Models;

namespace npascu_api_v1.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}