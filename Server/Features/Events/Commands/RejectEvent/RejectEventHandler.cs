using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Enums;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Events.Commands.RejectEvent
{
    public class RejectEventHandler : IRequestHandler<RejectEventCommand>
    {
        private readonly AppDbContext _context;
        private readonly INotificationDispatcher _dispatcher;

        public RejectEventHandler(AppDbContext context, INotificationDispatcher dispatcher)
        {
            _context = context;
            _dispatcher = dispatcher;
        }

        public async Task Handle(RejectEventCommand request, CancellationToken cancellationToken)
        {
            var ev = await _context.Events
                .Include(e => e.Organizer)
                .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

            if (ev == null)
                throw new Exception(AppConstants.Errors.EVENT_NOT_FOUND);

            ev.Status = request.IsToFix ? EventStatus.Pending : EventStatus.Rejected;
            await _context.SaveChangesAsync(cancellationToken);

            if (ev.Organizer != null)
            {
                var reasonText = !string.IsNullOrWhiteSpace(request.Reason)
                    ? request.Reason
                    : AppConstants.Others.LOCATION_NOT_SPECIFIED;

                try
                {
                    var bodyTemplate = request.IsToFix
                        ? AppConstants.Success.NOTIFICATION_EVENT_REJECT_BODY
                        : AppConstants.Success.NOTIFICATION_EVENT_REJECTED_FINAL_BODY;

                    var subjectTemplate = request.IsToFix
                        ? AppConstants.Success.NOTIFICATION_EVENT_REJECT_SUBJ
                        : AppConstants.Success.NOTIFICATION_EVENT_REJECTED_FINAL_SUBJ;

                    var msg = string.Format(bodyTemplate, ev.Title, reasonText);

                    await _dispatcher.SendAsync(
                        userId: ev.Organizer.UserId,
                        message: msg,
                        emailSubject: subjectTemplate,
                        emailBody: msg,
                        cancellationToken: cancellationToken
                    );
                }
                catch (FormatException ex)
                {
                    throw new InvalidOperationException(AppConstants.Errors.INTERNAL_SERVER_ERROR, ex);
                }
            }
        }
    }
}