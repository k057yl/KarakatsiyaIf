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
            var category = await _context.EventCategories
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (category == null)
            {
                throw new InvalidOperationException(AppConstants.Errors.CATEGORY_NOT_EXIST);
            }

            _context.EventCategories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
