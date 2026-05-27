using MediatR;

namespace Karakatsiya.Features.Admin.Commands.DismissReport
{
    public record DismissReportCommand(Guid CommentId) : IRequest;
}
