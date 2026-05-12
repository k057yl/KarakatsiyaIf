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

            await context.Database.MigrateAsync();

            if (!await context.Users.AnyAsync(u => u.Role == UserRole.SuperAdmin))
            {
                var superAdmin = new User
                {
                    Email = "romank057yl@gmail.com",
                    PasswordHash = BC.HashPassword("Qwe_123"),
                    Role = UserRole.SuperAdmin
                };

                context.Users.Add(superAdmin);
                await context.SaveChangesAsync();

                Console.WriteLine("✅ Суперадмин успешно создан!");
            }
        }
    }
}
