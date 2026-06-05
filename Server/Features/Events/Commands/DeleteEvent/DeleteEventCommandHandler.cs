using Karakatsiya.Constants;
using Karakatsiya.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Events.Commands.DeleteEvent
{
    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand>
    {
        private readonly AppDbContext _context;

        public DeleteEventCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            var @event = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (@event == null)
                throw new KeyNotFoundException(AppConstants.Errors.EVENT_NOT_FOUND);

            @event.IsDeleted = true;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
