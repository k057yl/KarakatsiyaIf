using Karakatsiya.Models.Dtos.Event;
using MediatR;

namespace Karakatsiya.Features.Admin.Queries.GetActiveEvents
{
    public record GetActiveEventsQuery : IRequest<List<AdminActiveEventDto>>;
}
