using Karakatsiya.Features.Admin.Dtos;
using MediatR;

namespace Karakatsiya.Features.Admin.Queries.GetPendingOrganizers
{
    public record GetPendingOrganizersQuery() : IRequest<List<PendingOrganizerDto>>;
}
