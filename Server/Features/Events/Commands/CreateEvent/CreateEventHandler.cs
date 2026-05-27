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
                throw new Exception(AppConstants.Errors.ORGANIZER_NOT_FOUND);
            }

            var p = request.Payload;
            var cleanTitle = _sanitizer.StripAllHtml(p.Title);
            var safeDescription = _sanitizer.SanitizeHtml(p.Description);

            Location? location = null;

            if (!string.IsNullOrEmpty(p.OsmId))
            {
                location = await _context.Set<Location>()
                    .FirstOrDefaultAsync(l => l.OsmId == p.OsmId, cancellationToken);
            }

            if (location == null)
            {
                location = new Location
                {
                    Name = p.LocationName,
                    OsmId = p.OsmId,
                    IsVerified = false,
                    Address = new Address(
                        City: p.City,
                        Street: p.Street,
                        HouseNumber: p.HouseNumber,
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
                ContactLinks = p.ContactLinks
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
            var slug = title.ToLower()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace(".", "");

            return $"{slug}-{Guid.NewGuid().ToString()[..6]}";
        }
    }
}