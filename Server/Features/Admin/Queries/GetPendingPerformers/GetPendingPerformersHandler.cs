using Karakatsiya.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Queries.GetPendingPerformers
{
    public class GetPendingPerformersHandler : IRequestHandler<GetPendingPerformersQuery, List<PendingPerformerViewModel>>
    {
        private readonly AppDbContext _context;

        public GetPendingPerformersHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PendingPerformerViewModel>> Handle(GetPendingPerformersQuery request, CancellationToken cancellationToken)
        {
            return await _context.Performers
                .AsNoTracking()
                .Where(p => !p.IsVerified)
                .Select(p => new PendingPerformerViewModel(p.Id, p.Name, p.Slug))
                .ToListAsync(cancellationToken);
        }
    }
}
