using Karakatsiya.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Commands.DeleteCommentByReport
{
    public class DeleteCommentByReportHandler : IRequestHandler<DeleteCommentByReportCommand>
    {
        private readonly AppDbContext _db;

        public DeleteCommentByReportHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task Handle(DeleteCommentByReportCommand request, CancellationToken ct)
        {
            var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == request.CommentId, ct);
            if (comment != null)
            {
                _db.Comments.Remove(comment);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
