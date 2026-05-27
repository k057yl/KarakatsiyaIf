using Karakatsiya.Models.Entities.Audience;
using Karakatsiya.Models.Entities.Common;

namespace Karakatsiya.Models.Entities.Admin
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
