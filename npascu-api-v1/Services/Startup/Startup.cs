using Microsoft.EntityFrameworkCore;
using npascu_api_v1.Repository;
using npascu_api_v1.Repository.Implementation;
using npascu_api_v1.Repository.Interface;
using npascu_api_v1.Services.Implementation;
using npascu_api_v1.Services.Interface;

namespace npascu_api_v1.Services.Startup
{
    public class Startup
    {
        public void AddDbContext(WebApplicationBuilder builder)
        {
            var defaultConnString = "";
            var assembly = typeof(Startup).Assembly.FullName;

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    defaultConnString = builder.Configuration.GetConnectionString("DefaultConnection");
                }
                else
                {
                    defaultConnString = builder.Configuration.GetConnectionString("npascu-api-v1");
                }

                options.UseSqlServer(defaultConnString);
            });
        }

        public void AddServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IItemRepository, ItemRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}
