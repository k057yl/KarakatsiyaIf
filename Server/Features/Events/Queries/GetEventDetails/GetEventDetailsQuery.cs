using Karakatsiya.Models.Dtos.Event;
using MediatR;

namespace Karakatsiya.Features.Events.Queries.GetEventDetails
{
    public record GetEventDetailsQuery(Guid Id) : IRequest<EventDetailsDto>;
}
