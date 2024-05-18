using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class update_require_name_document : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Folder_Name",
                table: "Folder");

            migrationBuilder.DropIndex(
                name: "IX_File_Name",
                table: "File");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Folder_Name",
                table: "Folder",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_File_Name",
                table: "File",
                column: "Name",
                unique: true);
        }
    }
}
