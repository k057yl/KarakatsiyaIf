namespace Karakatsiya.Models.Dtos.Comment
{
    public record CreateCommentRequest(Guid EventId, string Text, bool ShowInstagram, bool ShowTelegram);
}
