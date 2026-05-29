using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Enums;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Events.Commands.ApproveEvent
{
    public class ApproveEventHandler : IRequestHandler<ApproveEventCommand>
    {
        private readonly AppDbContext _context;
        private readonly INotificationDispatcher _dispatcher;

        public ApproveEventHandler(AppDbContext context, INotificationDispatcher dispatcher)
        {
            _context = context;
            _dispatcher = dispatcher;
        }

        public async Task Handle(ApproveEventCommand request, CancellationToken cancellationToken)
        {
            var ev = await _context.Events
                .Include(e => e.Organizer)
                .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

            if (ev == null)
                throw new Exception(AppConstants.Errors.EVENT_NOT_FOUND);

            ev.Status = EventStatus.Approved;
            ev.IsVip = request.IsVip;

            await _context.SaveChangesAsync(cancellationToken);

            if (ev.Organizer != null)
            {
                var text = string.Format(AppConstants.Success.NOTIFICATION_EVENT_APPROVED_BODY, ev.Title);
                if (request.IsVip)
                {
                    text += AppConstants.Success.NOTIFICATION_EVENT_APPROVED_VIP;
                }

                await _dispatcher.SendAsync(
                    userId: ev.Organizer.UserId,
                    message: text,
                    emailSubject: AppConstants.Success.NOTIFICATION_EVENT_APPROVED_SUBJ,
                    emailBody: text,
                    cancellationToken: cancellationToken
                );
            }
        }
    }
}