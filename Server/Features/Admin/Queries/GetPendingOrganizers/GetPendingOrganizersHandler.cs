using Karakatsiya.Data;
using Karakatsiya.Data.Enums;
using Karakatsiya.Features.Admin.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Queries.GetPendingOrganizers
{
    public class GetPendingOrganizersHandler : IRequestHandler<GetPendingOrganizersQuery, List<PendingOrganizerDto>>
    {
        private readonly AppDbContext _context;

        public GetPendingOrganizersHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PendingOrganizerDto>> Handle(GetPendingOrganizersQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Role == UserRole.PendingOrganizer && u.OrganizerProfile != null)
                .Select(u => new PendingOrganizerDto(
                    u.Id,
                    u.OrganizerProfile!.Id,
                    u.OrganizerProfile.Name,
                    u.OrganizerProfile.Contacts.Phone,
                    u.OrganizerProfile.Contacts.Email,
                    u.OrganizerProfile.Contacts.Telegram,
                    u.OrganizerProfile.CreatedAt
                ))
                .ToListAsync(cancellationToken);
                
        }
    }
}
