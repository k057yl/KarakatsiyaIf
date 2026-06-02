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

            if (realOrganizerId == Guid.Empty) return false;

            var ev = await _context.Events
                .Include(e => e.Location)
                .FirstOrDefaultAsync(e => e.Id == request.EventId && e.OrganizerId == realOrganizerId, cancellationToken);

            if (ev == null) return false;

            ev.Title = _sanitizer.StripAllHtml(request.Title);
            ev.Description = _sanitizer.SanitizeHtml(request.Description);
            ev.StartDate = request.StartDate.ToUniversalTime();
            ev.Status = EventStatus.Pending;

            if (ev.Location != null)
            {
                ev.Location.Name = string.IsNullOrWhiteSpace(request.LocationName) ? AppConstants.General.NOT_NAME : request.LocationName;
                ev.Location.OsmId = request.OsmId;

                ev.Location.Address = new Address(
                    request.City ?? string.Empty,
                    request.Street ?? string.Empty,
                    request.HouseNumber ?? string.Empty,
                    request.Latitude,
                    request.Longitude
                );
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}