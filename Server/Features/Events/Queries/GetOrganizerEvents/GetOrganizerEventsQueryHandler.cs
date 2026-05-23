using Karakatsiya.Data;
using Karakatsiya.Models.Dtos.Organizer;
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
            var realOrganizerId = await _context.Organizers
                .Where(o => o.UserId == request.OrganizerId)
                .Select(o => o.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (realOrganizerId == Guid.Empty)
            {
                return new List<OrganizerEventDto>();
            }

            return await _context.Events
                .AsNoTracking()
                .Include(e => e.Location)
                .Where(e => e.OrganizerId == realOrganizerId)
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
                    City = e.Location != null && e.Location.Address != null ? e.Location.Address.City : string.Empty,
                    Street = e.Location != null && e.Location.Address != null ? e.Location.Address.Street : string.Empty,
                    HouseNumber = e.Location != null && e.Location.Address != null ? e.Location.Address.HouseNumber : null,
                    Latitude = e.Location != null && e.Location.Address != null ? e.Location.Address.Latitude : null,
                    Longitude = e.Location != null && e.Location.Address != null ? e.Location.Address.Longitude : null,
                    OsmId = e.Location != null ? e.Location.OsmId : null
                })
                .ToListAsync(cancellationToken);
        }
    }
}