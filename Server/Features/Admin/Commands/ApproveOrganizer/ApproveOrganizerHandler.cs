using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Data.Enums;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Karakatsiya.Features.Admin.Commands.ApproveOrganizer
{
    public class ApproveOrganizerHandler : IRequestHandler<ApproveOrganizerCommand, Unit>
    {
        private readonly AppDbContext _context;
        private readonly INotificationDispatcher _dispatcher;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ApproveOrganizerHandler(AppDbContext context, INotificationDispatcher dispatcher, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _dispatcher = dispatcher;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(ApproveOrganizerCommand request, CancellationToken cancellationToken)
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

            organizer.User.Role = UserRole.Organizer;
            await _context.SaveChangesAsync(cancellationToken);

            var tgMessage = _localizer[AppConstants.Success.NOTIFICATION_ORG_APPROVED_BODY].Value;

            var emailSubjectKey = "EMAIL_ORGANIZER_APPROVED_SUBJECT";
            var emailBodyKey = "EMAIL_ORGANIZER_APPROVED_BODY";

            await _dispatcher.SendAsync(
                userId: organizer.UserId,
                message: tgMessage,
                emailSubject: emailSubjectKey,
                emailBody: emailBodyKey,
                cancellationToken: cancellationToken
            );

            return Unit.Value;
        }
    }
}