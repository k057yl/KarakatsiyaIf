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
        private readonly INotificationDispatcher _dispatcher;

        public RejectOrganizerHandler(AppDbContext context, INotificationDispatcher dispatcher)
        {
            _context = context;
            _dispatcher = dispatcher;
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

            var userId = organizer.UserId;
            organizer.User.Role = UserRole.Visitor;
            _context.Organizers.Remove(organizer);

            await _context.SaveChangesAsync(cancellationToken);

            var msg = string.Format(AppConstants.Success.NOTIFICATION_ORG_REJECT_BODY, string.Empty, request.Reason);

            await _dispatcher.SendAsync(
                userId: userId,
                message: msg,
                emailSubject: AppConstants.Success.NOTIFICATION_ORG_REJECT_SUBJ,
                emailBody: msg,
                cancellationToken: cancellationToken
            );

            return Unit.Value;
        }
    }
}