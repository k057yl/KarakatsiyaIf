using Karakatsiya.Features.Events.Dtos;
using MediatR;

namespace Karakatsiya.Features.Events.Queries.GetEventDetails
{
    public record GetEventDetailsQuery(Guid Id) : IRequest<EventDetailsDto>;
}
