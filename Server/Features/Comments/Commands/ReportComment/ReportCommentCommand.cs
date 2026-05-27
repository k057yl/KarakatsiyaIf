using MediatR;

namespace Karakatsiya.Features.Comments.Commands.ReportComment
{
    public record ReportCommentCommand(Guid CommentId, Guid ReporterId, string Reason)
        : IRequest<(bool Success, string MessageKey)>;
}
