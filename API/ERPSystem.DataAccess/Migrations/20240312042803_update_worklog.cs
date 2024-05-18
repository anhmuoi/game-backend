using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERPSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class update_worklog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkLog_Category",
                table: "WorkLog");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "WorkLog");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "WorkLog",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkLog_CategoryId",
                table: "WorkLog",
                newName: "IX_WorkLog_UserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDateRefreshToken",
                table: "Account",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "WorkSchedule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    FolderLogId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkSchedule_Category",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkSchedule_Folder",
                        column: x => x.FolderLogId,
                        principalTable: "FolderLog",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedule_CategoryId",
                table: "WorkSchedule",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedule_FolderLogId",
                table: "WorkSchedule",
                column: "FolderLogId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkLog_User",
                table: "WorkLog",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkLog_User",
                table: "WorkLog");

            migrationBuilder.DropTable(
                name: "WorkSchedule");

            migrationBuilder.DropColumn(
                name: "CreateDateRefreshToken",
                table: "Account");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "WorkLog",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkLog_UserId",
                table: "WorkLog",
                newName: "IX_WorkLog_CategoryId");

            migrationBuilder.AddColumn<short>(
                name: "Type",
                table: "WorkLog",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkLog_Category",
                table: "WorkLog",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
