using Microsoft.EntityFrameworkCore;
using npascu_api_v1.Models.Entities.Auth;
using npascu_api_v1.Models.Entities;

namespace npascu_api_v1.Repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
              
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
