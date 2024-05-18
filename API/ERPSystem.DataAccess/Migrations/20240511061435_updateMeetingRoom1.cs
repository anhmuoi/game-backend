using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateMeetingRoom1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentPeople",
                table: "MeetingRoom",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsRunning",
                table: "MeetingRoom",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "MeetingRoom",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "TotalPeople",
                table: "MeetingRoom",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPeople",
                table: "MeetingRoom");

            migrationBuilder.DropColumn(
                name: "IsRunning",
                table: "MeetingRoom");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "MeetingRoom");

            migrationBuilder.DropColumn(
                name: "TotalPeople",
                table: "MeetingRoom");
        }
    }
}
