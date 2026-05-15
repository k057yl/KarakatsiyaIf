using Karakatsiya.Models.Dtos.Event;
using MediatR;

namespace Karakatsiya.Features.Events.Queries.GetPendingEvents
{
    public record GetPendingEventsQuery() : IRequest<List<EventDto>>;
}
