using Karakatsiya.Data;
using Karakatsiya.Data.Enums;
using Karakatsiya.Features.Events.Dtos;
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
            var now = DateTime.UtcNow;

            return await _context.Events
                .AsNoTracking()
                .AsSplitQuery()
                .Where(e => e.Status == EventStatus.Approved && e.StartDate >= now)
                .OrderBy(e => e.StartDate)
                .Select(e => new EventDto(
                    e.Id,
                    e.Title,
                    e.StartDate,
                    e.Location != null ? e.Location.Name : Constants.AppConstants.General.NOT_NAME,
                    e.Location != null ? (e.Location.Address.City ?? string.Empty) : string.Empty,
                    e.Location != null ? (e.Location.Address.Street ?? string.Empty) : string.Empty,
                    e.Location != null ? (e.Location.Address.HouseNumber ?? string.Empty) : string.Empty,
                    e.Location != null ? e.Location.Address.Latitude : null,
                    e.Location != null ? e.Location.Address.Longitude : null,
                    e.IsVip,
                    e.Photos.Where(p => p.IsMain).Select(p => p.ImageUrl).FirstOrDefault()
                        ?? e.Photos.Select(p => p.ImageUrl).FirstOrDefault(),
                    e.CategoryId,
                    e.Category != null ? e.Category.Name : string.Empty,
                    e.EventPerformers
                        .Where(ep => ep.Performer != null)
                        .Select(ep => new PerformerMiniDto(
                            ep.Performer!.Id,
                            ep.Performer.Name,
                            ep.Performer.AvatarUrl
                        )).ToList()
                ))
                .ToListAsync(cancellationToken);
        }
    }
}