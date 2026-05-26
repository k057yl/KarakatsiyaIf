using MediatR;

namespace Karakatsiya.Features.Users.Commands.UpdateContacts
{
    public record UpdateContactsCommand(
        Guid UserId,
        string? Phone,
        string? Website,
        string? Telegram,
        string? Instagram
    ) : IRequest<(bool Success, string MessageKey)>;
}
