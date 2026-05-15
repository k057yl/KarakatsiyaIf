using Karakatsiya.Models.Entities.Common;
using Karakatsiya.Models.Entities.ValueObjects;

namespace Karakatsiya.Models.Entities.Showcase
{
    public class Location : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public Address Address { get; set; } = null!;

        public bool IsVerified { get; set; } = false;

        public string? OsmId { get; set; }
    }
}
