using Karakatsiya.Data.Entities.Common;
using Karakatsiya.Data.Entities.Showcase;

namespace Karakatsiya.Data.Entities.Audience
{
    public class Ticket : BaseEntity
    {
        public Guid EventId { get; set; }
        public Event? Event { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        public string TicketCode { get; set; } = string.Empty;
    }
}
