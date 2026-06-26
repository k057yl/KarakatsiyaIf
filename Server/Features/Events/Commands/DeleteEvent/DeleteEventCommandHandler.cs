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
            var updatedRows = await _context.Events
                .Where(e => e.Id == request.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.IsDeleted, true), cancellationToken);

            if (updatedRows == 0)
            {
                throw new KeyNotFoundException(AppConstants.Errors.EVENT_NOT_FOUND);
            }
        }
    }
}
