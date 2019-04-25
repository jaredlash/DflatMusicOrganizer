namespace Dflat.EF6.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFileFileChromaprintJobandFileMD5Jobentities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        FileID = c.Int(nullable: false, identity: true),
                        Filename = c.String(),
                        Extension = c.String(),
                        Directory = c.String(),
                        Size = c.Long(nullable: false),
                        LastModifiedTime = c.DateTime(nullable: false),
                        MD5Sum = c.String(),
                        Chromaprint = c.String(),
                    })
                .PrimaryKey(t => t.FileID);
            
            CreateTable(
                "dbo.FileChromaprintJobs",
                c => new
                    {
                        JobID = c.Int(nullable: false),
                        File_FileID = c.Int(),
                    })
                .PrimaryKey(t => t.JobID)
                .ForeignKey("dbo.Jobs", t => t.JobID)
                .ForeignKey("dbo.Files", t => t.File_FileID)
                .Index(t => t.JobID)
                .Index(t => t.File_FileID);
            
            CreateTable(
                "dbo.FileMD5Jobs",
                c => new
                    {
                        JobID = c.Int(nullable: false),
                        File_FileID = c.Int(),
                    })
                .PrimaryKey(t => t.JobID)
                .ForeignKey("dbo.Jobs", t => t.JobID)
                .ForeignKey("dbo.Files", t => t.File_FileID)
                .Index(t => t.JobID)
                .Index(t => t.File_FileID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FileMD5Jobs", "File_FileID", "dbo.Files");
            DropForeignKey("dbo.FileMD5Jobs", "JobID", "dbo.Jobs");
            DropForeignKey("dbo.FileChromaprintJobs", "File_FileID", "dbo.Files");
            DropForeignKey("dbo.FileChromaprintJobs", "JobID", "dbo.Jobs");
            DropIndex("dbo.FileMD5Jobs", new[] { "File_FileID" });
            DropIndex("dbo.FileMD5Jobs", new[] { "JobID" });
            DropIndex("dbo.FileChromaprintJobs", new[] { "File_FileID" });
            DropIndex("dbo.FileChromaprintJobs", new[] { "JobID" });
            DropTable("dbo.FileMD5Jobs");
            DropTable("dbo.FileChromaprintJobs");
            DropTable("dbo.Files");
        }
    }
}
