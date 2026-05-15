namespace Karakatsiya.Models.Dtos.Event
{
    public record CreateEventDto(
        string Title,
        string Description,
        DateTime StartDate,
        string LocationName,
        string City,
        string Street,
        string? HouseNumber,
        double? Latitude,
        double? Longitude,
        string? OsmId,
        string? ExternalTicketUrl,
        string? ContactLinks
    );
}
