using Karakatsiya.Data;
using Karakatsiya.Models.Dtos.Event;
using Karakatsiya.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Events.Queries.GetApprovedEvents
{
    public class GetApprovedEventsHandler : IRequestHandler<GetApprovedEventsQuery, List<EventDto>>
    {
        private readonly AppDbContext _context;
        public GetApprovedEventsHandler(AppDbContext context) => _context = context;

        public async Task<List<EventDto>> Handle(GetApprovedEventsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Events
                .Where(e => e.Status == EventStatus.Approved)
                .OrderBy(e => e.StartDate)
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
