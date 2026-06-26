namespace Karakatsiya.Features.Comments.Commands.Dtos
{
    public record CreateCommentRequest(Guid EventId, string Text, bool ShowInstagram, bool ShowTelegram);
}
