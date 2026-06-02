using Karakatsiya.Data.Entities.Admin;
using Karakatsiya.Data.Entities.Audience;
using Karakatsiya.Data.Entities.Common;
using Karakatsiya.Data.Enums;

namespace Karakatsiya.Data.Entities.Showcase
{
    public class Event : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public EventStatus Status { get; set; } = EventStatus.Draft;

        public Guid? CategoryId { get; set; }
        public EventCategory? Category { get; set; }

        public decimal? Price { get; set; }
        public int AgeRestriction { get; set; } = 0;

        public Guid LocationId { get; set; }
        public Location? Location { get; set; }

        public Guid OrganizerId { get; set; }
        public Organizer? Organizer { get; set; }

        public string? ExternalTicketUrl { get; set; }
        public string? ContactLinks { get; set; }

        public bool IsVip { get; set; } = false;
        public DateTime? VipExpiresAt { get; set; }
        public bool IsVipRequested { get; set; } = false;

        public List<EventServiceRequest> ServiceRequests { get; set; } = new();
        public List<Ticket> Tickets { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public List<EventPhoto> Photos { get; set; } = new();
        public int ViewsCount { get; set; } = 0;
    }
}