using Karakatsiya.Models.Entities.Common;

namespace Karakatsiya.Models.Entities.Audience
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
    }
}
