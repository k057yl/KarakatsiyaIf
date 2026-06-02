namespace Karakatsiya.Features.Events.Queries.GetAddressByCoords
{
    public record OsmAddressDto(
        string? City,
        string? Town,
        string? Village,
        string? Road,
        string? HouseNumber
    );
}
