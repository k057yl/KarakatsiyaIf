using Karakatsiya.Services.Interfaces;

namespace Karakatsiya.Services
{
    public class ConsoleEmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body)
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"📧 ПИСЬМО ДЛЯ: {to}");
            Console.WriteLine($"📝 ТЕМА: {subject}");
            Console.WriteLine($"💬 ТЕКСТ: {body}");
            Console.WriteLine("------------------------------------------");
            return Task.CompletedTask;
        }
    }
}
