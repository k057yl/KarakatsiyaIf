using Karakatsiya.Features.Comments.Commands.ReportComment;
using MediatR;

namespace Karakatsiya.Features.Admin.Queries.GetReportedComments
{
    public record GetReportedCommentsQuery : IRequest<List<ReportedCommentDto>>;
}
