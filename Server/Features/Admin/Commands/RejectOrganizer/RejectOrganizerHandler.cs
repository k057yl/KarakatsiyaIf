using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Enums;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Commands.RejectOrganizer
{
    public class RejectOrganizerHandler : IRequestHandler<RejectOrganizerCommand, Unit>
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public RejectOrganizerHandler(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<Unit> Handle(RejectOrganizerCommand request, CancellationToken cancellationToken)
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

            organizer.User.Role = UserRole.Visitor;

            _context.Organizers.Remove(organizer);

            await _context.SaveChangesAsync(cancellationToken);

            await _emailService.SendEmailAsync(
                organizer.User.Email,
                "EMAIL_ORGANIZER_REJECTED_SUBJECT",
                "EMAIL_ORGANIZER_REJECTED_BODY",
                request.Reason);

            return Unit.Value;
        }
    }
}
