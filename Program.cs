using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using npascu_api_v1.Data;
using npascu_api_v1.Modules.Background;
using npascu_api_v1.Modules.Hub;
using npascu_api_v1.Modules.Services;
using npascu_api_v1.Startup;

var builder = WebApplication.CreateBuilder(args);

// Add essential services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure CORS to allow any origin, method, and header.
// You can replace AllowAnyOrigin with WithOrigins("https://your-react-app.com") to restrict it.
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://pascu.io", "www.pascu.io")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add authentication
var jwtKey = builder.Configuration["JWT_KEY"] ?? throw new InvalidOperationException("JWT Key is missing");
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSignalR();
builder.Services.AddHttpClient<FinnhubRestService>();

builder.Services.AddHostedService<FinnhubRestService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// Get database connection string
var connectionString = ConnectionStringHelper.GetConnectionString(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();

// Enable CORS before other middleware
app.UseCors("Production");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<QuotesHub>("/quotesHub");

// Automatically apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var configuration = services.GetRequiredService<IConfiguration>();
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
    await DbInitializer.SeedAdminAsync(dbContext, configuration);
}

app.MapControllers();
app.Run();