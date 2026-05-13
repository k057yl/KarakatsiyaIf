using Karakatsiya.Models.Dtos.Organizer;
using MediatR;

namespace Karakatsiya.Features.Admin.Queries.GetPendingOrganizers
{
    public record GetPendingOrganizersQuery() : IRequest<List<PendingOrganizerDto>>;
}
