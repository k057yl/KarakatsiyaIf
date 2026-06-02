namespace Karakatsiya.Data.Entities.ValueObjects
{
    public record ContactInfo(
        string? Phone,
        string? Email,
        string? Website,
        string? Telegram,
        string? Instagram
    );
}
