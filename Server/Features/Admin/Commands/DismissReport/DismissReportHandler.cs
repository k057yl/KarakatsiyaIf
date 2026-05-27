using Karakatsiya.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Commands.DismissReport
{
    public class DismissReportHandler : IRequestHandler<DismissReportCommand>
    {
        private readonly AppDbContext _db;

        public DismissReportHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task Handle(DismissReportCommand request, CancellationToken ct)
        {
            var reports = await _db.CommentReports
                .Where(r => r.CommentId == request.CommentId && !r.IsResolved)
                .ToListAsync(ct);

            if (reports.Count != 0)
            {
                foreach (var report in reports)
                {
                    report.IsResolved = true;
                }
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
