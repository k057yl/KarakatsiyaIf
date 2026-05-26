using MediatR;

namespace Karakatsiya.Features.Comments.Commands.CreateComment
{
    public record CreateCommentCommand(
        Guid UserId,
        Guid EventId,
        string Text,
        bool ShowInstagram,
        bool ShowTelegram
    ) : IRequest<(bool Success, Guid? CommentId, string MessageKey)>;
}
