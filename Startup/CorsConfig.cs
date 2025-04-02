namespace npascu_api_v1.Startup;

public static class CorsExtensions
{
    public static void AddCustomCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", policyBuilder =>
            {
                policyBuilder
                    .WithOrigins(builder.Configuration["CorsOrigins"] ?? string.Empty)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
    }
}