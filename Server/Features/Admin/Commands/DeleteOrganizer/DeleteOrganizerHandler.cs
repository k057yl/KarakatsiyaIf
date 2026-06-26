using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Features.Admin.Commands.DeletePerformer;
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
            var deletedRows = await _context.Organizers
                .Where(c => c.Id == request.Id)
                .ExecuteDeleteAsync(cancellationToken);

            if (deletedRows == 0)
            {
                throw new InvalidOperationException(AppConstants.Errors.ORGANIZER_NOT_FOUND);
            }
        }
    }
}
