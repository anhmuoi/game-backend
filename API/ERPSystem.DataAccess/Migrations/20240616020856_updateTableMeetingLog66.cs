using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateTableMeetingLog66 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DepositDone",
                table: "MeetingLog",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepositDone",
                table: "MeetingLog");
        }
    }
}
