using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Enums;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Karakatsiya.Features.Events.Commands.ApproveEvent
{
    public class ApproveEventHandler : IRequestHandler<ApproveEventCommand>
    {
        private readonly AppDbContext _context;
        private readonly INotificationDispatcher _dispatcher;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ApproveEventHandler(AppDbContext context, INotificationDispatcher dispatcher, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _dispatcher = dispatcher;
            _localizer = localizer;
        }

        public async Task Handle(ApproveEventCommand request, CancellationToken cancellationToken)
        {
            var ev = await _context.Events
                .Include(e => e.Organizer)
                .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

            if (ev == null)
                throw new InvalidOperationException(AppConstants.Errors.EVENT_NOT_FOUND);

            ev.Status = EventStatus.Approved;
            ev.IsVip = request.IsVip;

            await _context.SaveChangesAsync(cancellationToken);

            if (ev.Organizer != null)
            {
                var tgTemplate = _localizer[AppConstants.Success.NOTIFICATION_EVENT_APPROVED_BODY].Value;
                var tgMessage = string.Format(tgTemplate, ev.Title);

                if (request.IsVip)
                {
                    tgMessage += _localizer[AppConstants.Success.NOTIFICATION_EVENT_APPROVED_VIP].Value;
                }

                var emailSubject = _localizer["EMAIL_EVENT_APPROVED_SUBJECT"].Value;
                var emailBodyTemplate = _localizer["EMAIL_EVENT_APPROVED_BODY"].Value;
                var emailBody = string.Format(emailBodyTemplate, ev.Title);

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