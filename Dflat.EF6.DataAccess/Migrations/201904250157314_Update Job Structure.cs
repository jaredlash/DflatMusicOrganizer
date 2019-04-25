namespace Dflat.EF6.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateJobStructure : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FileSourceFolderScanJobs", "FileSourceFolder_FileSourceFolderID", "dbo.FileSourceFolders");
            DropIndex("dbo.FileSourceFolderScanJobs", new[] { "FileSourceFolder_FileSourceFolderID" });
            RenameColumn(table: "dbo.FileSourceFolderScanJobs", name: "FileSourceFolder_FileSourceFolderID", newName: "FileSourceFolderID");
            AlterColumn("dbo.FileSourceFolderScanJobs", "FileSourceFolderID", c => c.Int(nullable: false));
            CreateIndex("dbo.FileSourceFolderScanJobs", "FileSourceFolderID");
            AddForeignKey("dbo.FileSourceFolderScanJobs", "FileSourceFolderID", "dbo.FileSourceFolders", "FileSourceFolderID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FileSourceFolderScanJobs", "FileSourceFolderID", "dbo.FileSourceFolders");
            DropIndex("dbo.FileSourceFolderScanJobs", new[] { "FileSourceFolderID" });
            AlterColumn("dbo.FileSourceFolderScanJobs", "FileSourceFolderID", c => c.Int());
            RenameColumn(table: "dbo.FileSourceFolderScanJobs", name: "FileSourceFolderID", newName: "FileSourceFolder_FileSourceFolderID");
            CreateIndex("dbo.FileSourceFolderScanJobs", "FileSourceFolder_FileSourceFolderID");
            AddForeignKey("dbo.FileSourceFolderScanJobs", "FileSourceFolder_FileSourceFolderID", "dbo.FileSourceFolders", "FileSourceFolderID");
        }
    }
}
