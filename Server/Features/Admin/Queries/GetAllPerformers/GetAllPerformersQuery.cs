using MediatR;

namespace Karakatsiya.Features.Admin.Queries.GetAllPerformers
{
    public record GetAllPerformersQuery(string? SearchTerm = null) : IRequest<List<AdminPerformerViewModel>>;
}
