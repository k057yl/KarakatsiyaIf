using Karakatsiya.Constants;
using Karakatsiya.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Commands.DeletePerformer
{
    public class DeletePerformerHandler : IRequestHandler<DeletePerformerCommand>
    {
        private readonly AppDbContext _context;

        public DeletePerformerHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeletePerformerCommand request, CancellationToken cancellationToken)
        {
            var performer = await _context.Performers
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (performer == null)
            {
                throw new InvalidOperationException(AppConstants.Errors.PERFORMER_NOT_FOUND);
            }

            _context.Performers.Remove(performer);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
