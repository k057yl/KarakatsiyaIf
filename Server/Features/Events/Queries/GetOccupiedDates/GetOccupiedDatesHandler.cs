using Karakatsiya.Data;
using Karakatsiya.Models.Enums;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace Karakatsiya.Features.Events.Queries.GetOccupiedDates
{
    public class GetOccupiedDatesHandler : IRequestHandler<GetOccupiedDatesQuery, List<string>>
    {
        private readonly AppDbContext _context;

        public GetOccupiedDatesHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> Handle(GetOccupiedDatesQuery request, CancellationToken cancellationToken)
        {
            var startDate = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endDate = startDate.AddMonths(1);

            var dates = await _context.Events
                .Where(e => e.Status == EventStatus.Approved &&
                            e.StartDate >= startDate &&
                            e.StartDate < endDate)
                .Select(e => e.StartDate)
                .ToListAsync(cancellationToken);

            return dates
                .Select(d => d.ToString("yyyy-MM-dd"))
                .Distinct()
                .ToList();
        }
    }
}
