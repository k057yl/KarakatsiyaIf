using System.Text;
using System.Text.RegularExpressions;
using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Data.Entities.Showcase;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Performers.Commands.CreatePerformer
{
    public class CreatePerformerHandler : IRequestHandler<CreatePerformerCommand, Guid>
    {
        private readonly AppDbContext _context;
        private readonly ISanitizerService _sanitizer;

        private static readonly string[] RusLetters = { "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ы", "ь", "э", "ю", "я", "і", "ї", "є", "ґ" };
        private static readonly string[] EngLetters = { "a", "b", "v", "g", "d", "e", "e", "zh", "z", "i", "y", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "f", "h", "ts", "ch", "sh", "shch", "", "y", "", "e", "yu", "ya", "i", "yi", "ye", "g" };

        public CreatePerformerHandler(AppDbContext context, ISanitizerService sanitizer)
        {
            _context = context;
            _sanitizer = sanitizer;
        }

        public async Task<Guid> Handle(CreatePerformerCommand request, CancellationToken cancellationToken)
        {
            var cleanName = _sanitizer.StripAllHtml(request.Name).Trim();

            if (string.IsNullOrWhiteSpace(cleanName))
            {
                throw new InvalidOperationException(AppConstants.Errors.PERFORMER_NAME_EMPTY);
            }

            var exists = await _context.Performers
                .AnyAsync(p => EF.Functions.ILike(p.Name, cleanName), cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(AppConstants.Errors.PERFORMER_ALREADY_EXISTS);
            }

            var newPerformer = new Performer
            {
                Id = Guid.NewGuid(),
                Name = cleanName,
                Slug = GenerateCleanSlug(cleanName),
                IsVerified = false
            };

            _context.Performers.Add(newPerformer);
            await _context.SaveChangesAsync(cancellationToken);

            return newPerformer.Id;
        }

        private static string GenerateCleanSlug(string title)
        {
            var text = title.ToLowerInvariant();
            var builder = new StringBuilder();
            foreach (var ch in text)
            {
                var index = Array.IndexOf(RusLetters, ch.ToString());
                builder.Append(index != -1 ? EngLetters[index] : ch.ToString());
            }

            var clean = Regex.Replace(builder.ToString(), @"[^a-z0-9\s-]", "");
            clean = Regex.Replace(clean, @"\s+", "-").Trim('-');

            return string.IsNullOrWhiteSpace(clean) ? AppConstants.General.SLUG_DEFAULT_PERFORMER : clean;
        }
    }
}