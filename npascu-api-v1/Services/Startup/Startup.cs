using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                }
                else
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("npascu-api-v1"));
                }

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

        public void AddSwaggerConfig(WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "NPASCU API V1", Version = "v1" });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
            });
        }
    }
}
