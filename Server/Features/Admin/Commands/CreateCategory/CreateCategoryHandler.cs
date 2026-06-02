using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Data.Entities.Showcase;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Commands.CreateCategory
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, EventCategory>
    {
        private readonly AppDbContext _context;

        public CreateCategoryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EventCategory> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Icon))
            {
                throw new InvalidOperationException(AppConstants.Errors.VALIDATION_FAILED);
            }

            var slug = request.Name.ToLower()
                .Trim()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace("\"", "");

            var exists = await _context.EventCategories.AnyAsync(c => c.Slug == slug, cancellationToken);
            if (exists)
            {
                throw new InvalidOperationException(AppConstants.Errors.CATEGORY_ALREADY_EXISTS);
            }

            var category = new EventCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = slug,
                Icon = request.Icon
            };

            await _context.EventCategories.AddAsync(category, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return category;
        }
    }
}
