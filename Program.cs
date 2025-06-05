using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using npascu_api_v1.Modules.Quote;
using npascu_api_v1.Modules.Services.FinnHub;
using npascu_api_v1.Modules.Services.IexCloud;
using npascu_api_v1.Modules.Services.Polygon;
using npascu_api_v1.Modules.Services.YahooFinance;
using npascu_api_v1.Modules.Services.AlphaVantage;
using npascu_api_v1.Modules.Services.Token;
using npascu_api_v1.Startup;

var builder = WebApplication.CreateBuilder(args);
var jwtKey = builder.Configuration["JWT_KEY"] ?? throw new InvalidOperationException("JWT Key is missing");
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.AddCustomCors();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfig();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSignalR();
builder.Services.AddHttpClient<FinnHubRestService>();
builder.Services.AddHostedService<FinnHubRestService>();
builder.Services.AddHostedService<FinnHubWebSocketService>();
builder.Services.AddHttpClient<IexCloudRestService>();
builder.Services.AddHostedService<IexCloudRestService>();
builder.Services.AddHttpClient<PolygonRestService>();
builder.Services.AddHostedService<PolygonRestService>();
builder.Services.AddHttpClient<YahooFinanceRestService>();
builder.Services.AddHostedService<YahooFinanceRestService>();
builder.Services.AddHttpClient<AlphaVantageHistoricalService>();

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

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowSpecificOrigin");

app.MapHub<QuotesHub>("/quotesHub");

app.MapControllers();
app.Run();
