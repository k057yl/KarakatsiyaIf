using Karakatsiya.Constants;
using Karakatsiya.Models.Entities.Audience;
using Karakatsiya.Models.Enums;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace Karakatsiya.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            await context.Database.MigrateAsync();

            var adminEmail = config[AppConstants.Config.SEED_ADMIN_EMAIL];
            var adminPass = config[AppConstants.Config.SEED_ADMIN_PASSWORD];

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPass))
            {
                // Если забыл прописать в json — лучше упасть сразу, чем гадать
                throw new InvalidOperationException("Admin credentials missing in appsettings.json!");
            }

            if (!await context.Users.AnyAsync(u => u.Role == UserRole.SuperAdmin))
            {
                var superAdmin = new User
                {
                    Email = adminEmail,
                    PasswordHash = BC.HashPassword(adminPass),
                    Role = UserRole.SuperAdmin
                };

                context.Users.Add(superAdmin);
                await context.SaveChangesAsync();

                Console.WriteLine(AppConstants.SeedData.ADMIN_CREATED_LOG);
            }
        }
    }
}