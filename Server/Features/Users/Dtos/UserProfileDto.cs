using Karakatsiya.Data.Entities.ValueObjects;

namespace Karakatsiya.Features.Users.Dtos
{
    public record UserProfileDto(
        Guid Id,
        string Email,
        string? Nickname,
        long? TelegramChatId,
        ContactInfo? Contacts
    );
}
