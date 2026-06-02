namespace Karakatsiya.Features.Comments.Commands.CreateComment
{
    public record CreateCommentRequest(Guid EventId, string Text, bool ShowInstagram, bool ShowTelegram);
}
