using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using npascu_api_v1.Repository;
using npascu_api_v1.Repository.Implementation;
using npascu_api_v1.Repository.Interface;
using npascu_api_v1.Services.Implementation;
using npascu_api_v1.Services.Interface;
using System.Text;

namespace npascu_api_v1.Services.Startup
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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

        public void AddAuthenticationConfig(WebApplicationBuilder builder)
        {

            var jwtIssuer = "";
            var jwtAudience = "";
            var jwtSecret = "";

            if (builder.Environment.IsDevelopment())
            {
                var section = _configuration.GetSection("JwtSettingsDev");
                jwtIssuer = section["Issuer"];
                jwtAudience = section["Audience"];
                jwtSecret = section["SecretKey"];
            }
            else
            {
                var section = _configuration.GetSection("JwtSettingsProd");
                jwtIssuer = section["Issuer"];
                jwtAudience = section["Audience"];
                jwtSecret = section["SecretKey"];
            }

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,        // YourIssuer
                    ValidIssuer = jwtIssuer,   // Replace with your issuer
                    ValidateAudience = true,      // YourAudience
                    ValidAudience = jwtAudience, // Replace with your audience
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // You can adjust the clock skew as needed
                };
            });
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
