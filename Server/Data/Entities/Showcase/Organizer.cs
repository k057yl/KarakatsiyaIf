using Karakatsiya.Data.Entities.Audience;
using Karakatsiya.Data.Entities.Common;
using Karakatsiya.Data.Entities.ValueObjects;

namespace Karakatsiya.Data.Entities.Showcase
{
    public class Organizer : BaseEntity
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public string Name { get; set; } = string.Empty;
        public ContactInfo Contacts { get; set; } = null!;
        public List<Event> Events { get; set; } = new();
    }
}
