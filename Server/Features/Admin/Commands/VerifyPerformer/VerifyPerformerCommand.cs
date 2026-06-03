using MediatR;

namespace Karakatsiya.Features.Admin.Commands.VerifyPerformer
{
    public record VerifyPerformerCommand(
        Guid Id,
        string Name,
        string? Description,
        string? AvatarUrl,
        string? InstagramUrl,
        string? TelegramUrl,
        string? YouTubeUrl
    ) : IRequest;
}
