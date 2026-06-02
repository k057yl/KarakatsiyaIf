using Karakatsiya.Data.Entities.Showcase;
using Karakatsiya.Data.Enums;

namespace Karakatsiya.Data.Entities.Admin
{
    public class EventServiceRequest
    {
        public Guid EventId { get; set; }
        public Event? Event { get; set; }
        public Guid AdminServiceId { get; set; }
        public AdminService? AdminService { get; set; }

        public decimal AgreedPrice { get; set; }
        public ServiceStatus Status { get; set; } = ServiceStatus.Requested;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
