using MediatR;

namespace Karakatsiya.Features.Events.Commands.DeleteEvent
{
    public record DeleteEventCommand(Guid Id) : IRequest;
}
