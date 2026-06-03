using MediatR;

namespace Karakatsiya.Features.Admin.Queries.GetPendingPerformers
{
    public record GetPendingPerformersQuery : IRequest<List<PendingPerformerViewModel>>;
}
