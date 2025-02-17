using npascu_api_v1.Data.Models;
using npascu_api_v1.Modules.Services;

namespace npascu_api_v1.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAdminAsync(ApplicationDbContext context, IConfiguration configuration)
        {
            var adminEmail = configuration["ADMIN_EMAIL"];
            var adminPassword = configuration["ADMIN_PASSWORD"];

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
                return;

            if (context.Users.Any(u => u.Email.Equals(adminEmail)))
                return;

            var adminUser = new User
            {
                Name = "admin",
                Email = adminEmail,
                PasswordHash = PasswordHelper.HashPassword(adminPassword)
            };

            var adminRole = new Role
            {
                Name = "Admin"
            };

            var userRole = new Role
            {
                Name = "User"
            };

            var adminUserRole = new UserRole
            {
                User = adminUser,
                Role = adminRole
            };

            context.Roles.Add(adminRole);
            context.Roles.Add(userRole);

            context.Users.Add(adminUser);

            context.UserRoles.Add(adminUserRole);
            await context.SaveChangesAsync();
        }
    }
}