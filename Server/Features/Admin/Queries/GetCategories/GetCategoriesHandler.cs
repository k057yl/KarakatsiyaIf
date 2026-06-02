using Karakatsiya.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Queries.GetCategories
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, List<CategoryViewModel>>
    {
        private readonly AppDbContext _context;

        public GetCategoriesHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryViewModel>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.EventCategories
                .AsNoTracking()
                .Select(c => new CategoryViewModel(c.Id, c.Name, c.Slug, c.Icon))
                .ToListAsync(cancellationToken);
        }
    }
}
