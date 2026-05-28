using MediatR;

namespace Karakatsiya.Features.Events.Queries.GetOccupiedDates
{
    public record GetOccupiedDatesQuery(int Year, int Month) : IRequest<List<string>>;
}
