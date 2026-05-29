using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Dtos.Category;
using Karakatsiya.Models.Entities.Showcase;
using Karakatsiya.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Controllers
{
    [ApiController]
    [Route("api/events/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var categories = await _context.EventCategories
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Slug,
                    c.Icon
                })
                .ToListAsync(cancellationToken);

            return Ok(categories);
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.SuperAdmin))]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Icon))
            {
                return BadRequest(new { Message = AppConstants.Errors.VALIDATION_FAILED });
            }

            var slug = dto.Name.ToLower()
                .Trim()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace("\"", "");

            var exists = await _context.EventCategories.AnyAsync(c => c.Slug == slug, cancellationToken);
            if (exists)
            {
                return BadRequest(new { Message = "Такая категория уже существует." });
            }

            var category = new EventCategory
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Slug = slug,
                Icon = dto.Icon
            };

            await _context.EventCategories.AddAsync(category, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(category);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = nameof(UserRole.SuperAdmin))]
        public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
        {
            var category = await _context.EventCategories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (category == null)
            {
                return NotFound();
            }

            _context.EventCategories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
