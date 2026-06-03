using System.Text;
using System.Text.RegularExpressions;
using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Data.Entities.Showcase;
using Karakatsiya.Data.Enums;
using Karakatsiya.Data.Entities.Audience;
using Karakatsiya.Data.Entities.ValueObjects;
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

            if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
            {
                var categoryExists = await _context.EventCategories
                    .AnyAsync(c => c.Id == request.CategoryId.Value, cancellationToken);

                if (!categoryExists)
                {
                    throw new InvalidOperationException(AppConstants.Errors.CATEGORY_NOT_EXIST);
                }
            }

            var cleanTitle = _sanitizer.StripAllHtml(request.Title);
            var safeDescription = _sanitizer.SanitizeHtml(request.Description);

            Location? location = null;

            if (!string.IsNullOrEmpty(request.OsmId))
            {
                location = await _context.Set<Location>()
                    .FirstOrDefaultAsync(l => l.OsmId == request.OsmId, cancellationToken);
            }

            if (location == null && (!string.IsNullOrWhiteSpace(request.LocationName) || !string.IsNullOrWhiteSpace(request.City)))
            {
                location = new Location
                {
                    Name = string.IsNullOrWhiteSpace(request.LocationName) ? AppConstants.General.NOT_NAME : request.LocationName,
                    OsmId = request.OsmId,
                    IsVerified = false,
                    Address = new Address(
                        City: request.City ?? string.Empty,
                        Street: request.Street ?? string.Empty,
                        HouseNumber: request.HouseNumber ?? string.Empty,
                        Latitude: request.Latitude,
                        Longitude: request.Longitude
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
                StartDate = request.StartDate.ToUniversalTime(),
                Status = EventStatus.Pending,
                Location = location,
                OrganizerId = organizer.Id,
                ExternalTicketUrl = request.ExternalTicketUrl,
                ContactLinks = request.ContactLinks,
                CategoryId = request.CategoryId
            };

            if (request.Photos != null && request.Photos.Any())
            {
                newEvent.Photos = request.Photos.Select(photo => new EventPhoto
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

            if (request.PerformerIds != null && request.PerformerIds.Any())
            {
                var validPerformerIds = await _context.Performers
                    .Where(p => request.PerformerIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .ToListAsync(cancellationToken);

                foreach (var performerId in validPerformerIds)
                {
                    newEvent.EventPerformers.Add(new EventPerformer
                    {
                        EventId = newEvent.Id,
                        PerformerId = performerId
                    });
                }
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