using Karakatsiya.Data;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Karakatsiya.Services.BackgroundServices
{
    public class TelegramBotHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TelegramBotHostedService> _logger;
        private readonly IConfiguration _configuration;
        private TelegramBotClient? _botClient;
        private CancellationTokenSource? _cts;

        public TelegramBotHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<TelegramBotHostedService> logger,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var token = _configuration["TelegramBotSettings:BotToken"];
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogError("Telegram BotToken отсутствует в конфигурации appsettings.json!");
                return Task.CompletedTask;
            }

            _botClient = new TelegramBotClient(token);
            _cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: _cts.Token
            );

            _logger.LogInformation("Фоновая служба Telegram-бота успешно запущена.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts?.Cancel();
            _logger.LogInformation("Фоновая служба Telegram-бота остановлена.");
            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken ct)
        {
            if (update.Message is not { Text: { } messageText } message) return;

            var chatId = message.Chat.Id;

            _logger.LogInformation("Получено сообщение от ChatId {ChatId}: {Text}", chatId, messageText);

            if (messageText.StartsWith("/start"))
            {
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "Привет! Я бот афиши Каракатица 🎸\n\nЧтобы привязать свой аккаунт и получать пуш-уведомления, отправь мне 6-значный код верификации из твоего личного кабинета на сайте.",
                    cancellationToken: ct
                );
                return;
            }

            var trimmedCode = messageText.Trim();
            if (trimmedCode.Length == 6 && int.TryParse(trimmedCode, out _))
            {
                var isLinked = await TryLinkUserByCodeAsync(chatId, trimmedCode);

                if (isLinked)
                {
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: "🎉 Отлично! Твой Telegram успешно привязан к аккаунту на сайте. Теперь ты будешь получать самые сочные пуши прямо сюда.",
                        cancellationToken: ct
                    );
                }
                else
                {
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: "❌ Неверный или просроченный код верификации. Сгенерируй новый код в профиле на сайте и попробуй ещё раз.",
                        cancellationToken: ct
                    );
                }
            }
            else
            {
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "🤔 Не понимаю эту команду. Отправь мне 6-значный цифровой код из профиля, чтобы связать аккаунты.",
                    cancellationToken: ct
                );
            }
        }

        private async Task<bool> TryLinkUserByCodeAsync(long chatId, string code)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var connectionCode = await db.TelegramConnectionCodes
                .FirstOrDefaultAsync(c => c.Code == code && c.ExpiryTime > DateTime.UtcNow);

            if (connectionCode == null) return false;

            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == connectionCode.UserId);
            if (user == null) return false;

            user.TelegramChatId = chatId;

            db.TelegramConnectionCodes.Remove(connectionCode);

            await db.SaveChangesAsync();
            return true;
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken ct)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogError(exception, "Ошибка пуллинга Telegram бота из источника {Source}: {ErrorMessage}", source, errorMessage);
            return Task.CompletedTask;
        }
    }
}
