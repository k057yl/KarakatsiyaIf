using Karakatsiya.Features.Events.Dtos;
using MediatR;

namespace Karakatsiya.Features.Events.Queries.GetApprovedEvents
{
    public record GetApprovedEventsQuery() : IRequest<List<EventDto>>;
}
