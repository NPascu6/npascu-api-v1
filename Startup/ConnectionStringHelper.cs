namespace npascu_api_v1.Startup;

public static class ConnectionStringHelper
{
    public static string GetConnectionString(IConfiguration configuration)
    {
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        var fallbackDatabaseUrl = configuration.GetConnectionString("DefaultConnection");

        if (!string.IsNullOrEmpty(databaseUrl))
        {
            return ParseDatabaseUrl(databaseUrl);
        }

        if (!string.IsNullOrEmpty(fallbackDatabaseUrl))
        {
            return ParseDatabaseUrl(fallbackDatabaseUrl);
        }

        throw new InvalidOperationException("No valid database connection string found.");
    }

    private static string ParseDatabaseUrl(string databaseUrl)
    {
        try
        {
            databaseUrl = databaseUrl.Replace("postgresql://", "postgres://");
            var uri = new Uri(databaseUrl);
            var userInfo = uri.UserInfo.Split(':');

            return
                $"Host={uri.Host};Port={5432};Username={userInfo[0]};Password={userInfo[1]};Database={uri.AbsolutePath.TrimStart('/')};SSL Mode=Require;Trust Server Certificate=True;";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing database URL: {ex.Message}");
            throw;
        }
    }
}