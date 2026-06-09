using Karakatsiya.Features.Events.Queries.GetAddressByCoords;
using MediatR;

namespace Karakatsiya.Features.Geo.Queries
{
    public record GetAddressByCoordsQuery(double Latitude, double Longitude) : IRequest<OsmReverseResponseDto>;
}
