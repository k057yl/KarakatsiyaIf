using Karakatsiya.Models.Entities.Admin;
using Karakatsiya.Models.Entities.Audience;
using Karakatsiya.Models.Entities.Common;
using Karakatsiya.Models.Enums;

namespace Karakatsiya.Models.Entities.Showcase
{
    public class Event : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public EventStatus Status { get; set; } = EventStatus.Draft;

        public Guid LocationId { get; set; }
        public Location? Location { get; set; }
        public Guid OrganizerId { get; set; }
        public Organizer? Organizer { get; set; }

        public List<EventServiceRequest> ServiceRequests { get; set; } = new();
        public List<Ticket> Tickets { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public List<EventPhoto> Photos { get; set; } = new();
    }
}
