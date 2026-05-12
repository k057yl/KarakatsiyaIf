using Karakatsiya.Models.Entities.Audience;
using Karakatsiya.Models.Entities.Common;
using Karakatsiya.Models.Entities.ValueObjects;

namespace Karakatsiya.Models.Entities.Showcase
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
