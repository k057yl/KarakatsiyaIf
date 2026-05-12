namespace Karakatsiya.Models.Entities.ValueObjects
{
    public record Address(
        string City,
        string Street,
        string? HouseNumber,
        double? Latitude,
        double? Longitude
    );
}
