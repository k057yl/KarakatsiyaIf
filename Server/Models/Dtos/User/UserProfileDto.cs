using Karakatsiya.Models.Entities.ValueObjects;

namespace Karakatsiya.Models.Dtos.User
{
    public record UserProfileDto(
        Guid Id,
        string Email,
        string? Nickname,
        long? TelegramChatId,
        ContactInfo? Contacts
    );
}
