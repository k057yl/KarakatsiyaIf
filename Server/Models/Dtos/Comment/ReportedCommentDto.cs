namespace Karakatsiya.Models.Dtos.Comment
{
    public record ReportedCommentDto(
        Guid ReportId,
        Guid CommentId,
        string CommentText,
        string AuthorName,
        string ReporterName,
        string Reason,
        DateTime ReportedAt
    );
}
