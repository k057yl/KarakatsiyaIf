using Karakatsiya.Data;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Services.BackgroundServices
{
    public class UnconfirmedUserCleanupWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UnconfirmedUserCleanupWorker> _logger;

        public UnconfirmedUserCleanupWorker(IServiceProvider serviceProvider, ILogger<UnconfirmedUserCleanupWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Ассасин вышел на охоту за 'половинчатыми' юзерами...");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var cutOffTime = DateTime.UtcNow.AddDays(-1);

                    var unconfirmedUsers = await db.Users
                        .Where(u => !u.IsEmailVerified && u.CreatedAt < cutOffTime)
                        .ToListAsync(stoppingToken);

                    if (unconfirmedUsers.Any())
                    {
                        db.Users.RemoveRange(unconfirmedUsers);
                        await db.SaveChangesAsync(stoppingToken);
                        _logger.LogWarning("Удалено {Count} мусорных аккаунтов.", unconfirmedUsers.Count);
                    }
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
