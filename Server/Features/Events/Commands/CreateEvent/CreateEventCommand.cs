using Karakatsiya.Models.Dtos.Event;
using MediatR;

namespace Karakatsiya.Features.Events.Commands.CreateEvent
{
    public record CreateEventCommand(Guid UserId, CreateEventDto Payload) : IRequest<Guid>;
}
