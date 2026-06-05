using Karakatsiya.Data;
using Karakatsiya.Data.Enums;
using Karakatsiya.Features.Events.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Events.Queries.GetPendingEvents
{
    public class GetPendingEventsHandler : IRequestHandler<GetPendingEventsQuery, List<EventDto>>
    {
        private readonly AppDbContext _context;
        public GetPendingEventsHandler(AppDbContext context) => _context = context;

        public async Task<List<EventDto>> Handle(GetPendingEventsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(e => e.Photos)
                .Where(e => e.Status == EventStatus.Pending)
                .OrderBy(e => e.CreatedAt)
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
                    e.IsVipRequested,
                    e.Photos.Where(p => p.IsMain).Select(p => p.ImageUrl).FirstOrDefault()
                        ?? e.Photos.Select(p => p.ImageUrl).FirstOrDefault()
                ))
                .ToListAsync(cancellationToken);
        }
    }
}