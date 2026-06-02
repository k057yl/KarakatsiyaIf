namespace Karakatsiya.Features.Events.Dtos
{
    public record EventCommentDto(
        Guid Id,
        string AuthorName,
        string Text,
        DateTime CreatedAt,
        string? InstagramUrl,
        string? TelegramUsername
    );
}
