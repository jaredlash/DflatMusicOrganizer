using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dflat.EFCore.DB.Migrations
{
    public partial class CreateFileSourceFolderrepo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileSourceFolders",
                columns: table => new
                {
                    FileSourceFolderID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Path = table.Column<string>(nullable: false),
                    IsTemporaryMedia = table.Column<bool>(nullable: false),
                    LastScanStart = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileSourceFolders", x => x.FileSourceFolderID);
                });

            migrationBuilder.CreateTable(
                name: "ExcludePaths",
                columns: table => new
                {
                    ExcludePathID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(nullable: false),
                    FileSourceFolderDataFileSourceFolderID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcludePaths", x => x.ExcludePathID);
                    table.ForeignKey(
                        name: "FK_ExcludePaths_FileSourceFolders_FileSourceFolderDataFileSourceFolderID",
                        column: x => x.FileSourceFolderDataFileSourceFolderID,
                        principalTable: "FileSourceFolders",
                        principalColumn: "FileSourceFolderID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExcludePaths_FileSourceFolderDataFileSourceFolderID",
                table: "ExcludePaths",
                column: "FileSourceFolderDataFileSourceFolderID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExcludePaths");

            migrationBuilder.DropTable(
                name: "FileSourceFolders");
        }
    }
}
