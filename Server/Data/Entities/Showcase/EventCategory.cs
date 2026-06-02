using Karakatsiya.Data.Entities.Common;

namespace Karakatsiya.Data.Entities.Showcase
{
    public class EventCategory : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Icon { get; set; }

        public List<Event> Events { get; set; } = new();
    }
}
