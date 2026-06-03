namespace Karakatsiya.Features.Events.Dtos
{
    public record EventDetailsPerformerDto(
        Guid Id,
        string Name,
        string Slug,
        string? AvatarUrl,
        string? Description,
        string? InstagramUrl,
        string? TelegramUrl,
        string? YoutubeUrl
    );
}
