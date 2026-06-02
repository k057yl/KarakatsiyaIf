namespace Karakatsiya.Features.Events.Dtos
{
    public record EventDto(Guid Id, string Title, DateTime StartDate, string LocationName, string City, string Street, string HouseNumber, double? Latitude, double? Longitude, bool IsVip);
}
