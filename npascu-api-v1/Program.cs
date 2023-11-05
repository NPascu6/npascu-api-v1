using Microsoft.EntityFrameworkCore;
using npascu_api_v1.Repository;
using npascu_api_v1.Repository.Implementation;
using npascu_api_v1.Repository.Interface;
using npascu_api_v1.Services.DB;
using npascu_api_v1.Services.Implementation;
using npascu_api_v1.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();


builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Apply database migrations
var migrator = new DatabaseMigrator(app.Services);
migrator.MigrateDatabase();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
