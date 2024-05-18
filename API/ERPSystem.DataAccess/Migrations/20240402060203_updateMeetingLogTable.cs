using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateMeetingLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeetingLog_MeetingRoom",
                table: "MeetingLog");

            migrationBuilder.AlterColumn<int>(
                name: "MeetingRoomId",
                table: "MeetingLog",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingLog_MeetingRoom",
                table: "MeetingLog",
                column: "MeetingRoomId",
                principalTable: "MeetingRoom",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeetingLog_MeetingRoom",
                table: "MeetingLog");

            migrationBuilder.AlterColumn<int>(
                name: "MeetingRoomId",
                table: "MeetingLog",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingLog_MeetingRoom",
                table: "MeetingLog",
                column: "MeetingRoomId",
                principalTable: "MeetingRoom",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
