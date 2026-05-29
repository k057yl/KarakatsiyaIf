using MediatR;

namespace Karakatsiya.Features.Events.Commands.RejectEvent
{
    public record RejectEventCommand(Guid EventId, string? Reason, bool IsToFix) : IRequest;
}
