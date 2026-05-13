using MediatR;

namespace Karakatsiya.Features.Organizers.Commands.ApplyForOrganizer
{
    public record ApplyForOrganizerCommand(
        Guid UserId,
        string Name,
        string? Phone,
        string? Email,
        string? Website,
        string? Telegram,
        string? Instagram
    ) : IRequest<Guid>;
}
