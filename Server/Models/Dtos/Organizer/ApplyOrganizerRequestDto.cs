namespace Karakatsiya.Models.Dtos.Organizer
{
    public record ApplyOrganizerRequestDto(
        string Name,
        string? Phone,
        string? Email,
        string? Website,
        string? Telegram,
        string? Instagram
    );
}
