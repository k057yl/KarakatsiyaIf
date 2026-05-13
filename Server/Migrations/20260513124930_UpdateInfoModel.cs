using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karakatsiya.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInfoModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Contacts_Instagram",
                table: "Organizers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contacts_Telegram",
                table: "Organizers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contacts_Instagram",
                table: "Organizers");

            migrationBuilder.DropColumn(
                name: "Contacts_Telegram",
                table: "Organizers");
        }
    }
}
