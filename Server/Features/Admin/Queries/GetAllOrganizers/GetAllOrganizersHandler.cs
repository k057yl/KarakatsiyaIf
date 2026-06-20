using Karakatsiya.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Queries.GetAllOrganizers
{
    public class GetAllOrganizersHandler : IRequestHandler<GetAllOrganizersQuery, List<AdminOrganizerViewModel>>
    {
        private readonly AppDbContext _context;

        public GetAllOrganizersHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminOrganizerViewModel>> Handle(GetAllOrganizersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Organizers.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = $"%{request.SearchTerm.Trim()}%";
                query = query.Where(o => EF.Functions.ILike(o.Name, search) ||
                                         (o.Contacts.Email != null && EF.Functions.ILike(o.Contacts.Email, search)));
            }

            return await query
                .OrderBy(o => o.Name)
                .Select(o => new AdminOrganizerViewModel(
                    o.Id,
                    o.Name,
                    o.Contacts.Email,
                    o.Contacts.Phone,
                    o.UserId
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
