using Karakatsiya.Data;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Karakatsiya.Features.Events.Dtos;
using Karakatsiya.Data.Enums;

namespace Karakatsiya.Features.Events.Queries.GetArchivedEvents
{
    public class GetArchivedEventsHandler : IRequestHandler<GetArchivedEventsQuery, List<EventDto>>
    {
        private readonly AppDbContext _context;
        public GetArchivedEventsHandler(AppDbContext context) => _context = context;

        public async Task<List<EventDto>> Handle(GetArchivedEventsQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            return await _context.Events
                .AsNoTracking()
                .Where(e => e.Status == EventStatus.Approved && e.StartDate < now)
                .OrderByDescending(e => e.StartDate)
                .Select(e => new EventDto(
                    e.Id,
                    e.Title,
                    e.StartDate,
                    e.Location != null ? e.Location.Name : Constants.AppConstants.General.NOT_NAME,
                    e.Location != null ? e.Location.Address.City : string.Empty,
                    e.Location != null ? e.Location.Address.Street : string.Empty,
                    e.Location != null ? e.Location.Address.HouseNumber : string.Empty,
                    e.Location != null ? e.Location.Address.Latitude : null,
                    e.Location != null ? e.Location.Address.Longitude : null,
                    e.IsVip
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
