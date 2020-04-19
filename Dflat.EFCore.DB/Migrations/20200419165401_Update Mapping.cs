using Microsoft.EntityFrameworkCore.Migrations;

namespace Dflat.EFCore.DB.Migrations
{
    public partial class UpdateMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExcludePaths_FileSourceFolders_FileSourceFolderDataFileSourceFolderID",
                table: "ExcludePaths");

            migrationBuilder.DropIndex(
                name: "IX_ExcludePaths_FileSourceFolderDataFileSourceFolderID",
                table: "ExcludePaths");

            migrationBuilder.DropColumn(
                name: "FileSourceFolderDataFileSourceFolderID",
                table: "ExcludePaths");

            migrationBuilder.AddColumn<int>(
                name: "FileSourceFolderID",
                table: "ExcludePaths",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExcludePaths_FileSourceFolderID",
                table: "ExcludePaths",
                column: "FileSourceFolderID");

            migrationBuilder.AddForeignKey(
                name: "FK_ExcludePaths_FileSourceFolders_FileSourceFolderID",
                table: "ExcludePaths",
                column: "FileSourceFolderID",
                principalTable: "FileSourceFolders",
                principalColumn: "FileSourceFolderID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExcludePaths_FileSourceFolders_FileSourceFolderID",
                table: "ExcludePaths");

            migrationBuilder.DropIndex(
                name: "IX_ExcludePaths_FileSourceFolderID",
                table: "ExcludePaths");

            migrationBuilder.DropColumn(
                name: "FileSourceFolderID",
                table: "ExcludePaths");

            migrationBuilder.AddColumn<int>(
                name: "FileSourceFolderDataFileSourceFolderID",
                table: "ExcludePaths",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExcludePaths_FileSourceFolderDataFileSourceFolderID",
                table: "ExcludePaths",
                column: "FileSourceFolderDataFileSourceFolderID");

            migrationBuilder.AddForeignKey(
                name: "FK_ExcludePaths_FileSourceFolders_FileSourceFolderDataFileSourceFolderID",
                table: "ExcludePaths",
                column: "FileSourceFolderDataFileSourceFolderID",
                principalTable: "FileSourceFolders",
                principalColumn: "FileSourceFolderID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
