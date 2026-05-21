using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Dtos.Event;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Events.Queries.GetEventDetails
{
    public class GetEventDetailsHandler : IRequestHandler<GetEventDetailsQuery, EventDetailsDto>
    {
        private readonly AppDbContext _context;

        public GetEventDetailsHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EventDetailsDto> Handle(GetEventDetailsQuery request, CancellationToken cancellationToken)
        {
            var ev = await _context.Events
                .Include(e => e.Location)
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (ev == null)
            {
                throw new Exception(AppConstants.Errors.EVENT_NOT_FOUND);
            }

            return new EventDetailsDto(
                ev.Id,
                ev.Title,
                ev.Description,
                ev.StartDate,
                ev.Location != null ? ev.Location.Name : AppConstants.Others.LOCATION_NOT_SPECIFIED,
                ev.Location != null ? ev.Location.Address.City : string.Empty,
                ev.Location != null ? ev.Location.Address.Street : string.Empty,
                ev.Location != null ? ev.Location.Address.HouseNumber : string.Empty,
                ev.Location?.Address.Latitude,
                ev.Location?.Address.Longitude,
                ev.Organizer != null ? ev.Organizer.Name : AppConstants.Others.ORGANIZER_NOT_SPECIFIED,
                ev.ExternalTicketUrl,
                ev.ContactLinks,
                ev.IsVip
            );
        }
    }
}
