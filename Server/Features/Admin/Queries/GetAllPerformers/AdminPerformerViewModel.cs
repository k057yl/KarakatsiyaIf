namespace Karakatsiya.Features.Admin.Queries.GetAllPerformers
{
    public record AdminPerformerViewModel(
        Guid Id,
        string Name,
        string Slug,
        bool IsVerified,
        string? AvatarUrl,
        string? Description,
        string? InstagramUrl,
        string? TelegramUrl,
        string? YouTubeUrl
    );
}
