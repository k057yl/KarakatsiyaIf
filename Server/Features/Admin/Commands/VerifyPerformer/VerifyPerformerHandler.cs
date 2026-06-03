using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

namespace Karakatsiya.Features.Admin.Commands.VerifyPerformer
{
    public class VerifyPerformerHandler : IRequestHandler<VerifyPerformerCommand>
    {
        private readonly AppDbContext _context;
        private readonly ISanitizerService _sanitizer;

        public VerifyPerformerHandler(AppDbContext context, ISanitizerService sanitizer)
        {
            _context = context;
            _sanitizer = sanitizer;
        }

        public async Task Handle(VerifyPerformerCommand request, CancellationToken cancellationToken)
        {
            var performer = await _context.Performers
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (performer == null)
            {
                throw new InvalidOperationException(AppConstants.Errors.PERFORMER_NOT_FOUND);
            }

            var cleanName = _sanitizer.StripAllHtml(request.Name).Trim();

            performer.Name = cleanName;
            performer.Slug = GenerateCleanSlug(cleanName);
            performer.Description = request.Description;
            performer.AvatarUrl = request.AvatarUrl;
            performer.InstagramUrl = request.InstagramUrl;
            performer.TelegramUrl = request.TelegramUrl;
            performer.YouTubeUrl = request.YouTubeUrl;
            performer.IsVerified = true;

            await _context.SaveChangesAsync(cancellationToken);
        }

        private static string GenerateCleanSlug(string title)
        {
            string[] rus = { "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ы", "ь", "э", "ю", "я", "і", "ї", "є", "ґ" };
            string[] eng = { "a", "b", "v", "g", "d", "e", "e", "zh", "z", "i", "y", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "f", "h", "ts", "ch", "sh", "shch", "", "y", "", "e", "yu", "ya", "i", "yi", "ye", "g" };

            var text = title.ToLowerInvariant();
            var builder = new StringBuilder();
            foreach (var ch in text)
            {
                var index = Array.IndexOf(rus, ch.ToString());
                builder.Append(index != -1 ? eng[index] : ch.ToString());
            }

            var clean = Regex.Replace(builder.ToString(), @"[^a-z0-9\s-]", "");
            clean = Regex.Replace(clean, @"\s+", "-").Trim('-');

            return string.IsNullOrWhiteSpace(clean) ? "performer" : clean;
        }
    }
}
