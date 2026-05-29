namespace Karakatsiya.Models.Dtos.Event
{
    public record OsmAddressDto(
        string? City,
        string? Town,
        string? Village,
        string? Road,
        string? HouseNumber
    );
}
