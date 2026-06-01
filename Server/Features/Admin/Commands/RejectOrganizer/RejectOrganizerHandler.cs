using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Enums;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Karakatsiya.Features.Admin.Commands.RejectOrganizer
{
    public class RejectOrganizerHandler : IRequestHandler<RejectOrganizerCommand, Unit>
    {
        private readonly AppDbContext _context;
        private readonly INotificationDispatcher _dispatcher;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public RejectOrganizerHandler(AppDbContext context, INotificationDispatcher dispatcher, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _dispatcher = dispatcher;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(RejectOrganizerCommand request, CancellationToken cancellationToken)
        {
            var organizer = await _context.Organizers
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == request.OrganizerId, cancellationToken);

            if (organizer == null || organizer.User == null)
            {
                throw new InvalidOperationException(AppConstants.Errors.ORGANIZER_NOT_FOUND);
            }

            if (organizer.User.Role != UserRole.PendingOrganizer)
            {
                throw new InvalidOperationException(AppConstants.Errors.NOT_PENDING_ORGANIZER);
            }

            var userId = organizer.UserId;
            organizer.User.Role = UserRole.Visitor;
            _context.Organizers.Remove(organizer);

            await _context.SaveChangesAsync(cancellationToken);

            var localizedSubject = _localizer[AppConstants.Success.NOTIFICATION_ORG_REJECT_SUBJ].Value;
            var localizedBodyTemplate = _localizer[AppConstants.Success.NOTIFICATION_ORG_REJECT_BODY].Value;
            var finalMessage = string.Format(localizedBodyTemplate, string.Empty, request.Reason);

            await _dispatcher.SendAsync(
                userId: userId,
                message: finalMessage,
                emailSubject: localizedSubject,
                emailBody: finalMessage,
                cancellationToken: cancellationToken
            );

            return Unit.Value;
        }
    }
}