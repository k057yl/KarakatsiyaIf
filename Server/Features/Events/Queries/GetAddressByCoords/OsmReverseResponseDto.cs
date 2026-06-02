namespace Karakatsiya.Features.Events.Queries.GetAddressByCoords
{
    public record OsmReverseResponseDto(
        string DisplayName,
        OsmAddressDto Address
    );
}
