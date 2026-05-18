namespace Karakatsiya.Models.Dtos.Event
{
    public record EventDetailsDto(
        Guid Id,
        string Title,
        string Description,
        DateTime StartDate,
        string LocationName,
        string City,
        string Street,
        string? HouseNumber,
        double? Latitude,
        double? Longitude,
        string OrganizerName,
        string? ExternalTicketUrl,
        string? ContactLinks
    );
}
