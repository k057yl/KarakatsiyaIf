using Karakatsiya.Models.Entities.Common;
using Karakatsiya.Models.Entities.Showcase;

namespace Karakatsiya.Models.Entities.Audience
{
    public class EventPhoto : BaseEntity
    {
        public Guid EventId { get; set; }
        public Event? Event { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;

        public bool IsMain { get; set; } = false;
        public bool IsApproved { get; set; } = false;
    }
}
