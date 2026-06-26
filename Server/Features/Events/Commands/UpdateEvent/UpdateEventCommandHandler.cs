using Karakatsiya.Data.Entities.Showcase;
using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Data.Entities.ValueObjects;
using Karakatsiya.Data.Enums;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Events.Commands.UpdateEvent
{
    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, bool>
    {
        private readonly AppDbContext _context;
        private readonly ISanitizerService _sanitizer;

        public UpdateEventCommandHandler(AppDbContext context, ISanitizerService sanitizer)
        {
            _context = context;
            _sanitizer = sanitizer;
        }

        public async Task<bool> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            var realOrganizerId = await _context.Organizers
                .Where(o => o.UserId == request.OrganizerId)
                .Select(o => o.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (realOrganizerId == Guid.Empty)
                throw new InvalidOperationException(AppConstants.Errors.ORGANIZER_NOT_FOUND);

            var ev = await _context.Events
                .Include(e => e.Location)
                .FirstOrDefaultAsync(e => e.Id == request.EventId && e.OrganizerId == realOrganizerId, cancellationToken);

            if (ev == null)
                throw new KeyNotFoundException(AppConstants.Errors.EVENT_NOT_FOUND);

            ev.Title = _sanitizer.StripAllHtml(request.Title);
            ev.Description = _sanitizer.SanitizeHtml(request.Description);
            ev.StartDate = request.StartDate.ToUniversalTime();
            ev.Status = EventStatus.Pending;

            Location? newLocation = null;

            if (!string.IsNullOrEmpty(request.OsmId))
            {
                newLocation = await _context.Set<Location>()
                    .FirstOrDefaultAsync(l => l.OsmId == request.OsmId, cancellationToken);
            }

            if (newLocation == null && (!string.IsNullOrWhiteSpace(request.LocationName) || !string.IsNullOrWhiteSpace(request.City)))
            {
                newLocation = new Location
                {
                    Id = Guid.NewGuid(),
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
                _context.Set<Location>().Add(newLocation);
            }

            ev.Location = newLocation;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}