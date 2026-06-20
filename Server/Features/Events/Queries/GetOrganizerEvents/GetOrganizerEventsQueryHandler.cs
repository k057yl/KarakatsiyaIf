using Karakatsiya.Data;
using Karakatsiya.Features.Organizers.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Events.Queries.GetOrganizerEvents
{
    public class GetOrganizerEventsQueryHandler : IRequestHandler<GetOrganizerEventsQuery, List<OrganizerEventDto>>
    {
        private readonly AppDbContext _context;

        public GetOrganizerEventsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrganizerEventDto>> Handle(GetOrganizerEventsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Events
                .AsNoTracking()
                .Where(e => e.Organizer!.UserId == request.OrganizerId)
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new OrganizerEventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    Status = e.Status,
                    IsVip = e.IsVip,
                    LocationName = e.Location != null ? e.Location.Name : string.Empty,
                    City = e.Location != null ? e.Location.Address.City! : string.Empty,
                    Street = e.Location != null ? e.Location.Address.Street! : string.Empty,
                    HouseNumber = e.Location != null ? e.Location.Address.HouseNumber : null,
                    Latitude = e.Location != null ? e.Location.Address.Latitude : null,
                    Longitude = e.Location != null ? e.Location.Address.Longitude : null,
                    OsmId = e.Location != null ? e.Location.OsmId : null
                })
                .ToListAsync(cancellationToken);
        }
    }
}