using MediatR;

namespace Karakatsiya.Features.Events.Commands.ApproveEvent
{
    public record ApproveEventCommand(Guid EventId, bool IsVip) : IRequest;
}
