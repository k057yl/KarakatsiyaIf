using MediatR;

namespace Karakatsiya.Features.Admin.Queries.GetAllOrganizers
{
    public record GetAllOrganizersQuery(string? SearchTerm = null) : IRequest<List<AdminOrganizerViewModel>>;
}
