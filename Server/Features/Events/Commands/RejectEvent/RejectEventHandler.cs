using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Data.Enums;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Karakatsiya.Features.Events.Commands.RejectEvent
{
    public class RejectEventHandler : IRequestHandler<RejectEventCommand>
    {
        private readonly AppDbContext _context;
        private readonly INotificationDispatcher _dispatcher;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public RejectEventHandler(AppDbContext context, INotificationDispatcher dispatcher, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _dispatcher = dispatcher;
            _localizer = localizer;
        }

        public async Task Handle(RejectEventCommand request, CancellationToken cancellationToken)
        {
            var ev = await _context.Events
                .Include(e => e.Organizer)
                .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

            if (ev == null)
                throw new InvalidOperationException(AppConstants.Errors.EVENT_NOT_FOUND);

            ev.Status = request.IsToFix ? EventStatus.Pending : EventStatus.Rejected;
            await _context.SaveChangesAsync(cancellationToken);

            if (ev.Organizer != null)
            {
                var reasonText = !string.IsNullOrWhiteSpace(request.Reason)
                    ? request.Reason
                    : AppConstants.Others.LOCATION_NOT_SPECIFIED;

                var tgTemplateKey = request.IsToFix
                    ? AppConstants.Success.NOTIFICATION_EVENT_REJECT_BODY
                    : AppConstants.Success.NOTIFICATION_EVENT_REJECTED_FINAL_BODY;

                var tgTemplate = _localizer[tgTemplateKey].Value;
                var tgMessage = string.Format(tgTemplate, ev.Title, reasonText);
                var emailSubjKey = request.IsToFix ? "EMAIL_EVENT_REJECT_SUBJECT" : "EMAIL_EVENT_REJECTED_FINAL_SUBJECT";
                var emailBodyKey = request.IsToFix ? "EMAIL_EVENT_REJECT_BODY" : "EMAIL_EVENT_REJECTED_FINAL_BODY";

                var emailSubject = _localizer[emailSubjKey].Value;
                var emailBodyTemplate = _localizer[emailBodyKey].Value;
                var emailBody = string.Format(emailBodyTemplate, ev.Title, reasonText);

                await _dispatcher.SendAsync(
                    userId: ev.Organizer.UserId,
                    message: tgMessage,
                    emailSubject: emailSubject,
                    emailBody: emailBody,
                    cancellationToken: cancellationToken
                );
            }
        }
    }
}