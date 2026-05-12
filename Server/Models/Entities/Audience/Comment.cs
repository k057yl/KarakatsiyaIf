using Karakatsiya.Models.Entities.Common;
using Karakatsiya.Models.Entities.Showcase;

namespace Karakatsiya.Models.Entities.Audience
{
    public class Comment : BaseEntity
    {
        public Guid EventId { get; set; }
        public Event? Event { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
