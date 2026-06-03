namespace Karakatsiya.Features.Events.Dtos
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
        string? ContactLinks,
        bool IsVip,
        List<EventDetailsPhotoDto> Photos,
        List<EventCommentDto> Comments,
        int ViewsCount,
        List<EventDetailsPerformerDto> Performers
    );
}
