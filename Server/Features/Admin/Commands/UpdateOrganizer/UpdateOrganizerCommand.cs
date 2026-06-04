using MediatR;

namespace Karakatsiya.Features.Admin.Commands.UpdateOrganizer
{
    public record UpdateOrganizerCommand(
        Guid Id,
        string Name,
        string? Phone,
        string? Email,
        string? Website,
        string? Telegram,
        string? Instagram
    ) : IRequest;
}
