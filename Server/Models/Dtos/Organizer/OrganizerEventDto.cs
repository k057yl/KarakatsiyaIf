using Karakatsiya.Models.Enums;

namespace Karakatsiya.Models.Dtos.Organizer
{
    public record OrganizerEventDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = null!;
        public string Description { get; init; } = null!;
        public DateTime StartDate { get; init; }
        public EventStatus Status { get; init; }
        public bool IsVip { get; init; }
        public string LocationName { get; init; } = null!;
        public string City { get; init; } = null!;
        public string Street { get; init; } = null!;
        public string? HouseNumber { get; init; }
        public double? Latitude { get; init; }
        public double? Longitude { get; init; }
        public string? OsmId { get; init; }
    }
}
