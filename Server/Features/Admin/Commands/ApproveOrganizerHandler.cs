using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Commands
{
    public class ApproveOrganizerHandler : IRequestHandler<ApproveOrganizerCommand, Unit>
    {
        private readonly AppDbContext _context;

        public ApproveOrganizerHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ApproveOrganizerCommand request, CancellationToken cancellationToken)
        {
            var organizer = await _context.Organizers
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == request.OrganizerId, cancellationToken);

            if (organizer == null || organizer.User == null)
            {
                throw new Exception(AppConstants.Errors.ORGANIZER_NOT_FOUND);
            }

            if (organizer.User.Role != UserRole.PendingOrganizer)
            {
                throw new Exception(AppConstants.Errors.NOT_PENDING_ORGANIZER);
            }

            organizer.User.Role = UserRole.Organizer;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
