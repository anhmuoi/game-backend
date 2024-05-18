using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateDailyRPTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyReport_FolderLog",
                table: "DailyReport");

            migrationBuilder.DropForeignKey(
                name: "FK_DailyReport_Reporter",
                table: "DailyReport");

            migrationBuilder.AlterColumn<int>(
                name: "ReporterId",
                table: "DailyReport",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "FolderLogId",
                table: "DailyReport",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyReport_FolderLog",
                table: "DailyReport",
                column: "FolderLogId",
                principalTable: "FolderLog",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyReport_Reporter",
                table: "DailyReport",
                column: "ReporterId",
                principalTable: "Account",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyReport_FolderLog",
                table: "DailyReport");

            migrationBuilder.DropForeignKey(
                name: "FK_DailyReport_Reporter",
                table: "DailyReport");

            migrationBuilder.AlterColumn<int>(
                name: "ReporterId",
                table: "DailyReport",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FolderLogId",
                table: "DailyReport",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyReport_FolderLog",
                table: "DailyReport",
                column: "FolderLogId",
                principalTable: "FolderLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyReport_Reporter",
                table: "DailyReport",
                column: "ReporterId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
