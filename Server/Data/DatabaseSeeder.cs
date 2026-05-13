using Karakatsiya.Constants;
using Karakatsiya.Models.Entities.Audience;
using Karakatsiya.Models.Entities.Showcase;
using Karakatsiya.Models.Entities.ValueObjects;
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

            await SeedAdmin(context, config);
            await SeedPendingOrganizers(context);
        }

        private static async Task SeedAdmin(AppDbContext context, IConfiguration config)
        {
            var adminEmail = config[AppConstants.Config.SEED_ADMIN_EMAIL];
            var adminPass = config[AppConstants.Config.SEED_ADMIN_PASSWORD];

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPass))
                throw new InvalidOperationException("Admin credentials missing in appsettings.json!");

            if (!await context.Users.AnyAsync(u => u.Role == UserRole.SuperAdmin))
            {
                var superAdmin = new User
                {
                    Email = adminEmail,
                    PasswordHash = BC.HashPassword(adminPass),
                    Role = UserRole.SuperAdmin,
                    IsEmailVerified = true
                };

                context.Users.Add(superAdmin);
                await context.SaveChangesAsync();
                Console.WriteLine(AppConstants.SeedData.ADMIN_CREATED_LOG);
            }
        }

        private static async Task SeedPendingOrganizers(AppDbContext context)
        {
            if (await context.Users.AnyAsync(u => u.Role == UserRole.PendingOrganizer))
                return;

            var testPassword = BC.HashPassword("Password123!");

            var technoUser = new User
            {
                Email = "techno.raver@example.com",
                PasswordHash = testPassword,
                Role = UserRole.PendingOrganizer,
                IsEmailVerified = true,
                OrganizerProfile = new Organizer
                {
                    Name = "Kyiv Techno Community",
                    Contacts = new ContactInfo(
                        Phone: "+380931112233",
                        Email: "rave@kyiv.ua",
                        Website: "https://techno-kyiv.com",
                        Telegram: "@techno_admin",
                        Instagram: "kyiv_techno_pulse"
                    )
                }
            };

            var artUser = new User
            {
                Email = "gallery.owner@example.com",
                PasswordHash = testPassword,
                Role = UserRole.PendingOrganizer,
                IsEmailVerified = true,
                OrganizerProfile = new Organizer
                {
                    Name = "Арт-Простір 'Своя Стеля'",
                    Contacts = new ContactInfo(
                        Phone: "+380509998877",
                        Email: "art@space.ua",
                        Website: null,
                        Telegram: "@art_manager",
                        Instagram: "svoya_stelya_art"
                    )
                }
            };

            context.Users.AddRange(technoUser, artUser);
            await context.SaveChangesAsync();

            Console.WriteLine("✅ Тестовые организаторы (петрушки) успешно добавлены в очередь на модерацию!");
        }
    }
}   