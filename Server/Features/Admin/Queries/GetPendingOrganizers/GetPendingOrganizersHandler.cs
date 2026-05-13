using Karakatsiya.Data;
using Karakatsiya.Models.Dtos.Organizer;
using Karakatsiya.Models.Enums;
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
            var pendingUsers = await _context.Users
                .Include(u => u.OrganizerProfile)
                .Where(u => u.Role == UserRole.PendingOrganizer && u.OrganizerProfile != null)
                .ToListAsync(cancellationToken);

            return pendingUsers.Select(u => new PendingOrganizerDto(
                UserId: u.Id,
                OrganizerId: u.OrganizerProfile!.Id,
                Name: u.OrganizerProfile.Name,
                Phone: u.OrganizerProfile.Contacts.Phone,
                Email: u.OrganizerProfile.Contacts.Email,
                Telegram: u.OrganizerProfile.Contacts.Telegram,
                AppliedAt: u.OrganizerProfile.CreatedAt
            )).ToList();
        }
    }
}
