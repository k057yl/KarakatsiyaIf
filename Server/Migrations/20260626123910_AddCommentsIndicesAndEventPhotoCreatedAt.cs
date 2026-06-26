using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karakatsiya.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentsIndicesAndEventPhotoCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CommentReports_CommentId",
                table: "CommentReports");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_EventId",
                table: "Comments",
                newName: "idx_comments_event_id");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_OsmId",
                table: "Locations",
                column: "OsmId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_comment_reports_lookup",
                table: "CommentReports",
                columns: new[] { "CommentId", "ReporterId", "IsResolved" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Locations_OsmId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "idx_comment_reports_lookup",
                table: "CommentReports");

            migrationBuilder.RenameIndex(
                name: "idx_comments_event_id",
                table: "Comments",
                newName: "IX_Comments_EventId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_CommentId",
                table: "CommentReports",
                column: "CommentId");
        }
    }
}
