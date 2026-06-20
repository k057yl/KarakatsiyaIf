using Karakatsiya.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Queries.GetAllPerformers
{
    public class GetAllPerformersHandler : IRequestHandler<GetAllPerformersQuery, List<AdminPerformerViewModel>>
    {
        private readonly AppDbContext _context;

        public GetAllPerformersHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminPerformerViewModel>> Handle(GetAllPerformersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Performers.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = $"%{request.SearchTerm.Trim()}%";
                query = query.Where(p => EF.Functions.ILike(p.Name, search));
            }

            return await query
                .OrderBy(p => p.Name)
                .Select(p => new AdminPerformerViewModel(
                    p.Id,
                    p.Name,
                    p.Slug,
                    p.IsVerified,
                    p.AvatarUrl,
                    p.Description,
                    p.InstagramUrl,
                    p.TelegramUrl,
                    p.YouTubeUrl
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
