using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Entities.Showcase;
using Karakatsiya.Models.Entities.ValueObjects;
using Karakatsiya.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Events.Commands.CreateEvent
{
    public class CreateEventHandler : IRequestHandler<CreateEventCommand, Guid>
    {
        private readonly AppDbContext _context;

        public CreateEventHandler(AppDbContext context)
        {
            _context = context;
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
                Title = p.Title,
                Slug = GenerateSlug(p.Title),
                Description = p.Description,
                StartDate = p.StartDate.ToUniversalTime(),
                Status = EventStatus.Draft,
                Location = location,
                OrganizerId = organizer.Id,
                ExternalTicketUrl = p.ExternalTicketUrl,
                ContactLinks = p.ContactLinks
            };

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
