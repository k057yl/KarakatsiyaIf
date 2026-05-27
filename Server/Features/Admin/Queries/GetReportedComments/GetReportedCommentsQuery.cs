using Karakatsiya.Models.Dtos.Comment;
using MediatR;

namespace Karakatsiya.Features.Admin.Queries.GetReportedComments
{
    public record GetReportedCommentsQuery : IRequest<List<ReportedCommentDto>>;
}
