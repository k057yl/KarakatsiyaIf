using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Features.Admin.Commands.DeleteCategory;
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
            var deletedRows = await _context.Performers
                .Where(c => c.Id == request.Id)
                .ExecuteDeleteAsync(cancellationToken);

            if (deletedRows == 0)
            {
                throw new InvalidOperationException(AppConstants.Errors.PERFORMER_NOT_FOUND);
            }
        }
    }
}
