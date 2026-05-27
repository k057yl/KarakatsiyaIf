using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Dtos.Comment;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Queries.GetReportedComments
{
    public class GetReportedCommentsHandler : IRequestHandler<GetReportedCommentsQuery, List<ReportedCommentDto>>
    {
        private readonly AppDbContext _db;

        public GetReportedCommentsHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ReportedCommentDto>> Handle(GetReportedCommentsQuery request, CancellationToken ct)
        {
            return await _db.CommentReports
                .Include(r => r.Comment).ThenInclude(c => c.User)
                .Include(r => r.Reporter)
                .Where(r => !r.IsResolved)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReportedCommentDto(
                    r.Id,
                    r.CommentId,
                    r.Comment != null ? r.Comment.Text : AppConstants.Others.COMMENT_DELETE,
                    r.Comment != null && r.Comment.User != null ? (r.Comment.User.Nickname ?? r.Comment.User.Email) : AppConstants.Others.ANONIM,
                    r.Reporter != null ? (r.Reporter.Nickname ?? r.Reporter.Email) : AppConstants.Others.ANONIM,
                    r.Reason,
                    r.CreatedAt
                ))
                .ToListAsync(ct);
        }
    }
}
