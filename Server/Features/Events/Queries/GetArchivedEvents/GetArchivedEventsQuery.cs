using Karakatsiya.Features.Events.Dtos;
using MediatR;

namespace Karakatsiya.Features.Events.Queries.GetArchivedEvents
{
    public record GetArchivedEventsQuery() : IRequest<List<EventDto>>;
}
