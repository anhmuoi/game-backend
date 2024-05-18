using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateMeetingLog1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeetingLog_FolderLog",
                table: "MeetingLog");

            migrationBuilder.DropIndex(
                name: "IX_MeetingLog_FolderLogId",
                table: "MeetingLog");

            migrationBuilder.DropColumn(
                name: "UserList",
                table: "MeetingRoom");

            migrationBuilder.DropColumn(
                name: "FolderLogId",
                table: "MeetingLog");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "MeetingLog",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "MeetingLog",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "UserList",
                table: "MeetingLog",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserList",
                table: "MeetingLog");

            migrationBuilder.AddColumn<string>(
                name: "UserList",
                table: "MeetingRoom",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "MeetingLog",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "MeetingLog",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FolderLogId",
                table: "MeetingLog",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MeetingLog_FolderLogId",
                table: "MeetingLog",
                column: "FolderLogId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingLog_FolderLog",
                table: "MeetingLog",
                column: "FolderLogId",
                principalTable: "FolderLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
