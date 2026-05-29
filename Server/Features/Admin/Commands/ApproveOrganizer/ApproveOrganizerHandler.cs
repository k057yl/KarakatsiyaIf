using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Enums;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Commands.ApproveOrganizer
{
    public class ApproveOrganizerHandler : IRequestHandler<ApproveOrganizerCommand, Unit>
    {
        private readonly AppDbContext _context;
        private readonly INotificationDispatcher _dispatcher;

        public ApproveOrganizerHandler(AppDbContext context, INotificationDispatcher dispatcher)
        {
            _context = context;
            _dispatcher = dispatcher;
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

            var msg = AppConstants.Success.NOTIFICATION_ORG_APPROVED_BODY;

            await _dispatcher.SendAsync(
                userId: organizer.UserId,
                message: msg,
                emailSubject: AppConstants.Success.NOTIFICATION_ORG_APPROVED_SUBJ,
                emailBody: msg,
                cancellationToken: cancellationToken
            );

            return Unit.Value;
        }
    }
}