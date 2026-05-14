namespace Karakatsiya.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subjectKey, string bodyKey, params object[] args);
    }
}
