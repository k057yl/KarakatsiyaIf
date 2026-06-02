namespace Karakatsiya.Features.Comments.Commands.ReportComment
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
