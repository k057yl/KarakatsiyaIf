using Karakatsiya.Data;
using Karakatsiya.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Services.BackgroundServices
{
    public class UnconfirmedUserCleanupWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<UnconfirmedUserCleanupWorker> _logger;

        public UnconfirmedUserCleanupWorker(IServiceProvider services, ILogger<UnconfirmedUserCleanupWorker> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Воркер очистки запущен и готов к карательным операциям...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var now = DateTime.UtcNow;
                    var emailCutoff = now.AddHours(-24);
                    var pendingCutoff = now.AddDays(-7);
                    var usersToDelete = await context.Users
                        .Where(u => !u.IsEmailVerified 
                                    && u.CreatedAt < emailCutoff
                                    && u.Role != UserRole.SuperAdmin)
                        .ToListAsync(stoppingToken);

                    if (usersToDelete.Any())
                    {
                        context.Users.RemoveRange(usersToDelete);
                        _logger.LogInformation("Удалено {Count} неподтвержденных аккаунтов (боты/мусор).", usersToDelete.Count);
                    }

                    var usersToDowngrade = await context.Users
                        .Include(u => u.OrganizerProfile)
                        .Where(u => u.Role == UserRole.PendingOrganizer
                                    && u.OrganizerProfile != null
                                    && u.OrganizerProfile.CreatedAt < pendingCutoff)
                        .ToListAsync(stoppingToken);

                    if (usersToDowngrade.Any())
                    {
                        foreach (var user in usersToDowngrade)
                        {
                            _logger.LogInformation("Юзер {Email} не прошел фейсконтроль вовремя. Понижаем до Visitor.", user.Email);

                            user.Role = UserRole.Visitor;

                            if (user.OrganizerProfile != null)
                            {
                                context.Set<Karakatsiya.Models.Entities.Showcase.Organizer>().Remove(user.OrganizerProfile);
                            }
                        }
                    }

                    if (usersToDelete.Any() || usersToDowngrade.Any())
                    {
                        await context.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ой, бля! Ошибка во время зачистки базы.");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}