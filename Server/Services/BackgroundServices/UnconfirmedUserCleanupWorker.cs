using Karakatsiya.Data;
using Karakatsiya.Data.Entities.Showcase;
using Karakatsiya.Data.Enums;
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
            _logger.LogInformation("UnconfirmedUserCleanupWorker started successfully.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var now = DateTime.UtcNow;
                    var emailCutoff = now.AddHours(-24);
                    var pendingCutoff = now.AddDays(-7);
                    var photoCutoff = now.AddHours(-1);

                    var usersToDelete = await context.Users
                        .Where(u => !u.IsEmailVerified
                                    && u.CreatedAt < emailCutoff
                                    && u.Role != UserRole.SuperAdmin)
                        .ToListAsync(stoppingToken);

                    if (usersToDelete.Any())
                    {
                        context.Users.RemoveRange(usersToDelete);
                        _logger.LogInformation("Successfully removed {Count} unverified accounts.", usersToDelete.Count);
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
                            _logger.LogInformation("User {Email} pending status expired. Downgrading to Visitor.", user.Email);

                            user.Role = UserRole.Visitor;

                            if (user.OrganizerProfile != null)
                            {
                                context.Set<Organizer>().Remove(user.OrganizerProfile);
                            }
                        }
                    }

                    if (usersToDelete.Any() || usersToDowngrade.Any())
                    {
                        await context.SaveChangesAsync(stoppingToken);
                    }

                    var deletedPhotosCount = await context.EventPhotos
                        .Where(p => !p.IsApproved && p.CreatedAt < photoCutoff)
                        .ExecuteDeleteAsync(stoppingToken);

                    if (deletedPhotosCount > 0)
                    {
                        _logger.LogInformation("Garbage collector cleared {Count} unapproved zombie photos from database.", deletedPhotosCount);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during the database cleanup process.");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}