namespace Karakatsiya.Models.Dtos.Event
{
    public record OsmReverseResponseDto(
        string DisplayName,
        OsmAddressDto Address
    );
}
