using MediatR;

namespace Karakatsiya.Features.Events.Commands.UpdateEvent
{
    public record UpdateEventCommand(
        Guid EventId,
        Guid OrganizerId,
        string Title,
        string Description,
        DateTime StartDate,
        string LocationName,
        string City,
        string Street,
        string? HouseNumber,
        double? Latitude,
        double? Longitude,
        string? OsmId
    ) : IRequest<bool>;
}