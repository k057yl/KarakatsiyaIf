using MediatR;

namespace Karakatsiya.Features.Events.Commands.CreateEvent
{
    public record CreateEventCommand(
        Guid UserId,
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
        string? ContactLinks,
        Guid? CategoryId,
        List<NestedCreateEventPhotoDto> Photos,
        List<Guid>? PerformerIds
    ) : IRequest<Guid>;

    public record NestedCreateEventPhotoDto(string ImageUrl, string PublicId, bool IsMain);
}
