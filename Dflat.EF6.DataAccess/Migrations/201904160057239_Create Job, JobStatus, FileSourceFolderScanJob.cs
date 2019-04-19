namespace Dflat.EF6.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateJobJobStatusFileSourceFolderScanJob : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Jobs",
                c => new
                    {
                        JobID = c.Int(nullable: false, identity: true),
                        CreationTime = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        Description = c.String(),
                        IgnoreCache = c.Boolean(nullable: false),
                        Output = c.String(),
                        Errors = c.String(),
                    })
                .PrimaryKey(t => t.JobID);
            
            CreateTable(
                "dbo.FileSourceFolderScanJobs",
                c => new
                    {
                        JobID = c.Int(nullable: false),
                        FileSourceFolder_FileSourceFolderID = c.Int(),
                    })
                .PrimaryKey(t => t.JobID)
                .ForeignKey("dbo.Jobs", t => t.JobID)
                .ForeignKey("dbo.FileSourceFolders", t => t.FileSourceFolder_FileSourceFolderID)
                .Index(t => t.JobID)
                .Index(t => t.FileSourceFolder_FileSourceFolderID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FileSourceFolderScanJobs", "FileSourceFolder_FileSourceFolderID", "dbo.FileSourceFolders");
            DropForeignKey("dbo.FileSourceFolderScanJobs", "JobID", "dbo.Jobs");
            DropIndex("dbo.FileSourceFolderScanJobs", new[] { "FileSourceFolder_FileSourceFolderID" });
            DropIndex("dbo.FileSourceFolderScanJobs", new[] { "JobID" });
            DropTable("dbo.FileSourceFolderScanJobs");
            DropTable("dbo.Jobs");
        }
    }
}
