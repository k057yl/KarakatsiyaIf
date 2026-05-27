using MediatR;

namespace Karakatsiya.Features.Admin.Commands.DeleteCommentByReport
{
    public record DeleteCommentByReportCommand(Guid CommentId) : IRequest;
}
