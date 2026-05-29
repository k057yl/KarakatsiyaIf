namespace Karakatsiya.Models.Entities.Audience
{
    public class TelegramConnectionCode
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }
    }
}
