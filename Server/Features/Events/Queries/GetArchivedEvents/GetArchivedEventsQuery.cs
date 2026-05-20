using Karakatsiya.Models.Dtos.Event;
using MediatR;

namespace Karakatsiya.Features.Events.Queries.GetArchivedEvents
{
    public record GetArchivedEventsQuery() : IRequest<List<EventDto>>;
}
