using System.Text;
using System.Text.RegularExpressions;
using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Entities.Audience;
using Karakatsiya.Models.Entities.Showcase;
using Karakatsiya.Models.Entities.ValueObjects;
using Karakatsiya.Models.Enums;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Events.Commands.CreateEvent
{
    public class CreateEventHandler : IRequestHandler<CreateEventCommand, Guid>
    {
        private readonly AppDbContext _context;
        private readonly ISanitizerService _sanitizer;

        public CreateEventHandler(AppDbContext context, ISanitizerService sanitizer)
        {
            _context = context;
            _sanitizer = sanitizer;
        }

        public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var organizer = await _context.Organizers
                .FirstOrDefaultAsync(o => o.UserId == request.UserId, cancellationToken);

            if (organizer == null)
            {
                throw new InvalidOperationException(AppConstants.Errors.ORGANIZER_NOT_FOUND);
            }

            var p = request.Payload;

            if (p.CategoryId.HasValue && p.CategoryId.Value != Guid.Empty)
            {
                var categoryExists = await _context.EventCategories
                    .AnyAsync(c => c.Id == p.CategoryId.Value, cancellationToken);

                if (!categoryExists)
                {
                    throw new InvalidOperationException(AppConstants.Errors.CATEGORY_NOT_EXIST);
                }
            }

            var cleanTitle = _sanitizer.StripAllHtml(p.Title);
            var safeDescription = _sanitizer.SanitizeHtml(p.Description);

            Location? location = null;

            if (!string.IsNullOrEmpty(p.OsmId))
            {
                location = await _context.Set<Location>()
                    .FirstOrDefaultAsync(l => l.OsmId == p.OsmId, cancellationToken);
            }

            if (location == null && (!string.IsNullOrWhiteSpace(p.LocationName) || !string.IsNullOrWhiteSpace(p.City)))
            {
                location = new Location
                {
                    Name = string.IsNullOrWhiteSpace(p.LocationName) ? AppConstants.General.NOT_NAME : p.LocationName,
                    OsmId = p.OsmId,
                    IsVerified = false,
                    Address = new Address(
                        City: p.City ?? string.Empty,
                        Street: p.Street ?? string.Empty,
                        HouseNumber: p.HouseNumber ?? string.Empty,
                        Latitude: p.Latitude,
                        Longitude: p.Longitude
                    )
                };
                _context.Set<Location>().Add(location);
            }

            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Title = cleanTitle,
                Slug = GenerateSlug(cleanTitle),
                Description = safeDescription,
                StartDate = p.StartDate.ToUniversalTime(),
                Status = EventStatus.Pending,
                Location = location,
                OrganizerId = organizer.Id,
                ExternalTicketUrl = p.ExternalTicketUrl,
                ContactLinks = p.ContactLinks,
                CategoryId = p.CategoryId
            };

            if (p.Photos != null && p.Photos.Any())
            {
                newEvent.Photos = p.Photos.Select(photo => new EventPhoto
                {
                    Id = Guid.NewGuid(),
                    EventId = newEvent.Id,
                    UserId = request.UserId,
                    ImageUrl = photo.ImageUrl,
                    PublicId = photo.PublicId,
                    IsMain = photo.IsMain,
                    IsApproved = true
                }).ToList();
            }

            _context.Set<Event>().Add(newEvent);
            await _context.SaveChangesAsync(cancellationToken);

            return newEvent.Id;
        }

        private static string GenerateSlug(string title)
        {
            var transliterated = Transliterate(title.ToLowerInvariant());
            var clean = Regex.Replace(transliterated, @"[^a-z0-9\s-]", "");
            clean = Regex.Replace(clean, @"\s+", "-").Trim('-');

            if (string.IsNullOrWhiteSpace(clean))
            {
                clean = "event";
            }

            return $"{clean}-{Guid.NewGuid().ToString()[..6]}";
        }

        private static string Transliterate(string text)
        {
            string[] rus = { "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ы", "ь", "э", "ю", "я", "і", "ї", "є", "ґ" };
            string[] eng = { "a", "b", "v", "g", "d", "e", "e", "zh", "z", "i", "y", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "f", "h", "ts", "ch", "sh", "shch", "", "y", "", "e", "yu", "ya", "i", "yi", "ye", "g" };

            var builder = new StringBuilder();
            foreach (var ch in text)
            {
                var index = Array.IndexOf(rus, ch.ToString());
                if (index != -1)
                    builder.Append(eng[index]);
                else
                    builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}