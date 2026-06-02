using Karakatsiya.Features.Events.Dtos;
using MediatR;

namespace Karakatsiya.Features.Events.Queries.GetPendingEvents
{
    public record GetPendingEventsQuery() : IRequest<List<EventDto>>;
}
