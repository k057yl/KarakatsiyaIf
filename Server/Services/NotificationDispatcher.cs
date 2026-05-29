using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Entities.Audience;
using Karakatsiya.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace Karakatsiya.Services.Infrastructure
{
    public class NotificationDispatcher : INotificationDispatcher
    {
        private readonly AppDbContext _db;
        private readonly IEmailService _emailService;
        private readonly TelegramBotClient? _botClient;

        public NotificationDispatcher(AppDbContext db, IEmailService emailService, IConfiguration configuration)
        {
            _db = db;
            _emailService = emailService;

            var token = configuration[AppConstants.Config.TG_BOT_TOKEN];
            if (!string.IsNullOrWhiteSpace(token))
            {
                _botClient = new TelegramBotClient(token);
            }
        }

        public async Task SendAsync(Guid userId, string message, string emailSubject, string emailBody, CancellationToken cancellationToken)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Message = message,
                IsRead = false
            };
            await _db.Notifications.AddAsync(notification, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user != null && user.TelegramChatId.HasValue && _botClient != null)
            {
                try
                {
                    await _botClient.SendMessage(
                        chatId: user.TelegramChatId.Value,
                        text: message,
                        cancellationToken: cancellationToken
                    );
                    return;
                }
                catch
                {
                }
            }

            if (user != null)
            {
                await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
            }
        }
    }
}