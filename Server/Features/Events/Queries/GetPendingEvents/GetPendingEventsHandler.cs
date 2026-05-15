using Karakatsiya.Data;
using Karakatsiya.Models.Dtos.Event;
using Karakatsiya.Models.Enums;
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
                .Where(e => e.Status == EventStatus.Pending)
                .OrderBy(e => e.CreatedAt)
                .Select(e => new EventDto(
                    e.Id,
                    e.Title,
                    e.StartDate,
                    e.Location != null ? e.Location.Name : Constants.AppConstants.General.NOT_NAME,
                    e.Location != null ? e.Location.Address.City : string.Empty,
                    e.Location != null ? e.Location.Address.Street : string.Empty,
                    e.Location != null ? e.Location.Address.HouseNumber : string.Empty
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
