using Karakatsiya.Models.Dtos.Event;
using MediatR;

namespace Karakatsiya.Features.Events.Queries.GetApprovedEvents
{
    public record GetApprovedEventsQuery() : IRequest<List<EventDto>>;
}
