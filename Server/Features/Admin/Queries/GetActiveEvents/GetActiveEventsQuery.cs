using Karakatsiya.Features.Admin.Dtos;
using MediatR;

namespace Karakatsiya.Features.Admin.Queries.GetActiveEvents
{
    public record GetActiveEventsQuery : IRequest<List<AdminActiveEventDto>>;
}
