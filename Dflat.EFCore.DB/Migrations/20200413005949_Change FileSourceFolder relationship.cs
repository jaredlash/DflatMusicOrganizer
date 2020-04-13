using Microsoft.EntityFrameworkCore.Migrations;

namespace Dflat.EFCore.DB.Migrations
{
    public partial class ChangeFileSourceFolderrelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExcludePaths_FileSourceFolders_FileSourceFolderDataFileSourceFolderID",
                table: "ExcludePaths");

            migrationBuilder.AddForeignKey(
                name: "FK_ExcludePaths_FileSourceFolders_FileSourceFolderDataFileSourceFolderID",
                table: "ExcludePaths",
                column: "FileSourceFolderDataFileSourceFolderID",
                principalTable: "FileSourceFolders",
                principalColumn: "FileSourceFolderID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExcludePaths_FileSourceFolders_FileSourceFolderDataFileSourceFolderID",
                table: "ExcludePaths");

            migrationBuilder.AddForeignKey(
                name: "FK_ExcludePaths_FileSourceFolders_FileSourceFolderDataFileSourceFolderID",
                table: "ExcludePaths",
                column: "FileSourceFolderDataFileSourceFolderID",
                principalTable: "FileSourceFolders",
                principalColumn: "FileSourceFolderID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
