using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Data.Enums;
using Karakatsiya.Features.Admin.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Queries.GetActiveEvents
{
    public class GetActiveEventsHandler : IRequestHandler<GetActiveEventsQuery, List<AdminActiveEventDto>>
    {
        private readonly AppDbContext _context;

        public GetActiveEventsHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminActiveEventDto>> Handle(GetActiveEventsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Events
                .AsNoTracking()
                .Where(e => e.Status == EventStatus.Approved && e.StartDate >= DateTime.UtcNow)
                .OrderBy(e => e.StartDate)
                .Select(e => new AdminActiveEventDto(
                    e.Id,
                    e.Title,
                    e.StartDate,
                    e.Location != null ? e.Location.Name : AppConstants.General.NOT_NAME,
                    e.Location != null ? e.Location.Address.City : string.Empty,
                    e.IsVip
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
