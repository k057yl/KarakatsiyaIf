using Karakatsiya.Constants;
using Karakatsiya.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Commands.DeleteOrganizer
{
    public class DeleteOrganizerHandler : IRequestHandler<DeleteOrganizerCommand>
    {
        private readonly AppDbContext _context;

        public DeleteOrganizerHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteOrganizerCommand request, CancellationToken cancellationToken)
        {
            var organizer = await _context.Organizers
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

            if (organizer == null)
            {
                throw new InvalidOperationException(AppConstants.Errors.ORGANIZER_NOT_FOUND);
            }

            _context.Organizers.Remove(organizer);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
