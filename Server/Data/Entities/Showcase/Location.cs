using Karakatsiya.Data.Entities.Common;
using Karakatsiya.Data.Entities.ValueObjects;

namespace Karakatsiya.Data.Entities.Showcase
{
    public class Location : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public Address Address { get; set; } = null!;

        public bool IsVerified { get; set; } = false;

        public string? OsmId { get; set; }
    }
}
