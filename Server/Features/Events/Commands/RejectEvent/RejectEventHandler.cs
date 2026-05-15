using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Enums;
using MediatR;

namespace Karakatsiya.Features.Events.Commands.RejectEvent
{
    public class RejectEventHandler : IRequestHandler<RejectEventCommand>
    {
        private readonly AppDbContext _context;
        public RejectEventHandler(AppDbContext context) => _context = context;

        public async Task Handle(RejectEventCommand request, CancellationToken cancellationToken)
        {
            var ev = await _context.Events.FindAsync(new object[] { request.EventId }, cancellationToken);

            if (ev == null)
                throw new Exception(AppConstants.Errors.EVENT_NOT_FOUND);

            ev.Status = EventStatus.Rejected;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
