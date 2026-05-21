using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Enums;
using MediatR;

namespace Karakatsiya.Features.Events.Commands.ApproveEvent
{
    public class ApproveEventHandler : IRequestHandler<ApproveEventCommand>
    {
        private readonly AppDbContext _context;
        public ApproveEventHandler(AppDbContext context) => _context = context;

        public async Task Handle(ApproveEventCommand request, CancellationToken cancellationToken)
        {
            var ev = await _context.Events.FindAsync(new object[] { request.EventId }, cancellationToken);

            if (ev == null)
                throw new Exception(AppConstants.Errors.EVENT_NOT_FOUND);

            ev.Status = EventStatus.Approved;
            ev.IsVip = request.IsVip;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}