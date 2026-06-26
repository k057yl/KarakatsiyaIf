using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Data.Entities.Admin;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Comments.Commands.ReportComment
{
    public class ReportCommentCommandHandler : IRequestHandler<ReportCommentCommand, (bool Success, string MessageKey)>
    {
        private readonly AppDbContext _db;

        public ReportCommentCommandHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<(bool Success, string MessageKey)> Handle(ReportCommentCommand request, CancellationToken ct)
        {
            var commentExists = await _db.Comments.AnyAsync(c => c.Id == request.CommentId, ct);
            if (!commentExists) return (false, AppConstants.Errors.COMMENT_NOT_FOUND);

            var alreadyReported = await _db.CommentReports
                .AnyAsync(r => r.CommentId == request.CommentId
                            && r.ReporterId == request.ReporterId
                            && !r.IsResolved, ct);

            if (alreadyReported) return (true, AppConstants.Success.REQUEST_APPROVED);

            var report = new CommentReport
            {
                Id = Guid.NewGuid(),
                CommentId = request.CommentId,
                ReporterId = request.ReporterId,
                Reason = request.Reason,
                IsResolved = false
            };

            _db.CommentReports.Add(report);
            await _db.SaveChangesAsync(ct);

            return (true, AppConstants.Success.REQUEST_APPROVED);
        }
    }
}
