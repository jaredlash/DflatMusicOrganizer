using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dflat.EFCore.DB.Migrations
{
    public partial class CreateFileSourceFolderScanJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    JobID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    IgnoreCache = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Output = table.Column<string>(nullable: false),
                    Errors = table.Column<string>(nullable: false),
                    JobType = table.Column<int>(nullable: false),
                    FileSourceFolderID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.JobID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jobs");
        }
    }
}
