using Karakatsiya.Data;
using Karakatsiya.Models.Entities.ValueObjects;
using Karakatsiya.Models.Enums;
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

            ev.Title = _sanitizer.StripAllHtml(request.Payload.Title);
            ev.Description = _sanitizer.SanitizeHtml(request.Payload.Description);
            ev.StartDate = request.Payload.StartDate;
            ev.Status = EventStatus.Pending;

            if (ev.Location != null)
            {
                ev.Location.Name = request.Payload.LocationName;
                ev.Location.OsmId = request.Payload.OsmId;

                ev.Location.Address = new Address(
                    request.Payload.City,
                    request.Payload.Street,
                    request.Payload.HouseNumber,
                    request.Payload.Latitude,
                    request.Payload.Longitude
                );
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}