using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using npascu_api_v1.Repository;
using npascu_api_v1.Repository.Implementation;
using npascu_api_v1.Repository.Interface;
using npascu_api_v1.Services.Auth.Email.Implementation;
using npascu_api_v1.Services.Auth.Email.Interface;
using npascu_api_v1.Services.Implementation;
using npascu_api_v1.Services.Interface;
using System.Text;

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
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddHostedService<EmailValidationHostedService>();
        }

        public void ConfigureJWTForSwagger(WebApplicationBuilder builder)
        {
            var issuerSecret = builder.Configuration.GetSection("Authentication")
                .GetSection("Swagger")
                .GetSection("Secret").Value;
            var issuer = builder.Configuration.GetSection("Authentication")
                .GetSection("Swagger")
                .GetSection("Issuer").Value;
            var audience = builder.Configuration.GetSection("Authentication")
                .GetSection("Swagger")
                .GetSection("Audience").Value;

            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "npascu-api", Version = "v1" });
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
            if(issuerSecret != null)
            {
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = issuer,       // Replace with your issuer
                            ValidAudience = audience,   // Replace with your audience
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerSecret)) // Replace with your secret key
                        };
                    });
            }
        }
    }
}
