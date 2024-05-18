using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class update_delete_cascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkLog_Folder",
                table: "WorkLog");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedule_Folder",
                table: "WorkSchedule");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkLog_Folder",
                table: "WorkLog",
                column: "FolderLogId",
                principalTable: "FolderLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSchedule_Folder",
                table: "WorkSchedule",
                column: "FolderLogId",
                principalTable: "FolderLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkLog_Folder",
                table: "WorkLog");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedule_Folder",
                table: "WorkSchedule");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkLog_Folder",
                table: "WorkLog",
                column: "FolderLogId",
                principalTable: "FolderLog",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSchedule_Folder",
                table: "WorkSchedule",
                column: "FolderLogId",
                principalTable: "FolderLog",
                principalColumn: "Id");
        }
    }
}
