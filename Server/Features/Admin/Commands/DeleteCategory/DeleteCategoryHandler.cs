using Karakatsiya.Constants;
using Karakatsiya.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Commands.DeleteCategory
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly AppDbContext _context;

        public DeleteCategoryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var deletedRows = await _context.EventCategories
                .Where(c => c.Id == request.Id)
                .ExecuteDeleteAsync(cancellationToken);

            if (deletedRows == 0)
            {
                throw new InvalidOperationException(AppConstants.Errors.CATEGORY_NOT_EXIST);
            }
        }
    }
}
