using Karakatsiya.Models.Dtos.Event;
using MediatR;

namespace Karakatsiya.Features.Events.Commands.UpdateEvent
{
    public record UpdateEventCommand(Guid EventId, Guid OrganizerId, CreateEventDto Payload) : IRequest<bool>;
}
