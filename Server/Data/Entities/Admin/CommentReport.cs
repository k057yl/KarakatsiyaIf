using Karakatsiya.Data.Entities.Audience;
using Karakatsiya.Data.Entities.Common;

namespace Karakatsiya.Data.Entities.Admin
{
    public class CommentReport : BaseEntity
    {
        public Guid CommentId { get; set; }
        public Comment Comment { get; set; } = null!;

        public Guid ReporterId { get; set; }
        public User Reporter { get; set; } = null!;

        public string Reason { get; set; } = string.Empty;
        public bool IsResolved { get; set; } = false;
    }
}
