using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class update_FKSchedule_FolderLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedule_Category",
                table: "WorkSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedule_Folder",
                table: "WorkSchedule");

            migrationBuilder.AlterColumn<int>(
                name: "FolderLogId",
                table: "WorkSchedule",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "WorkSchedule",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSchedule_Category",
                table: "WorkSchedule",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSchedule_Folder",
                table: "WorkSchedule",
                column: "FolderLogId",
                principalTable: "FolderLog",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedule_Category",
                table: "WorkSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedule_Folder",
                table: "WorkSchedule");

            migrationBuilder.AlterColumn<int>(
                name: "FolderLogId",
                table: "WorkSchedule",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "WorkSchedule",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSchedule_Category",
                table: "WorkSchedule",
                column: "CategoryId",
                principalTable: "Category",
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
    }
}
