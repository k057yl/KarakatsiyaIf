using Karakatsiya.Data.Entities.Common;
using Karakatsiya.Data.Entities.Showcase;

namespace Karakatsiya.Data.Entities.Audience
{
    public class Comment : BaseEntity
    {
        public Guid EventId { get; set; }
        public Event? Event { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public string Text { get; set; } = string.Empty;

        public bool ShowInstagram { get; set; } = false;
        public bool ShowTelegram { get; set; } = false;
    }
}
