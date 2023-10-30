using Microsoft.EntityFrameworkCore;
using npascu_api_v1.Models.Entities;
using System.Collections.Generic;

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
        public DbSet<OrderItem> UserItems { get; set; }
    }
}
