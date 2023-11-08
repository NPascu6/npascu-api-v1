using npascu_api_v1.Services.DB;
using npascu_api_v1.Services.Startup;

var builder = WebApplication.CreateBuilder(args);
var startup = new Startup();

startup.AddDbContext(builder);
startup.AddServices(builder);
startup.ConfigureJWTForSwagger(builder);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();