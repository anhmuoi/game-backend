using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateTableMeetingLog156 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CreateBattleSuccess",
                table: "MeetingLog",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateBattleSuccess",
                table: "MeetingLog");
        }
    }
}
