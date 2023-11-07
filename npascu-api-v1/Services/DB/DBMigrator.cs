using Microsoft.EntityFrameworkCore;
using npascu_api_v1.Repository;

namespace npascu_api_v1.Services.DB
{
    public class DatabaseMigrator
    {
        private IServiceProvider _serviceProvider;

        public DatabaseMigrator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void MigrateDatabase()
        {
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<DatabaseMigrator>>();
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

                try
                {

                    dbContext.Database.Migrate();
                    logger.LogInformation("Database migration successful.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred during database migration.");
                }
            }
        }
    }
}
