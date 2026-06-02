using Karakatsiya.Features.Organizers.Dtos;
using MediatR;

namespace Karakatsiya.Features.Events.Queries.GetOrganizerEvents
{
    public record GetOrganizerEventsQuery(Guid OrganizerId) : IRequest<List<OrganizerEventDto>>;
}
