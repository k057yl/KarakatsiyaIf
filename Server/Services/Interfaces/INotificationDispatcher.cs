namespace Karakatsiya.Services.Interfaces
{
    public interface INotificationDispatcher
    {
        Task SendAsync(Guid userId, string message, string emailSubject, string emailBody, CancellationToken cancellationToken);
    }
}
