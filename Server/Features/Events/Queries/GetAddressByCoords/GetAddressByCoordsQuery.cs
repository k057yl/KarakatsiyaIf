using MediatR;

namespace Karakatsiya.Features.Events.Queries.GetAddressByCoords
{
    public record GetAddressByCoordsQuery(double Lat, double Lon) : IRequest<OsmReverseResponseDto?>;
}
