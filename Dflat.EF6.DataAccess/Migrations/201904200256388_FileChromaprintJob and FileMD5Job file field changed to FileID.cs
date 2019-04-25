namespace Dflat.EF6.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileChromaprintJobandFileMD5JobfilefieldchangedtoFileID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FileChromaprintJobs", "File_FileID", "dbo.Files");
            DropForeignKey("dbo.FileMD5Jobs", "File_FileID", "dbo.Files");
            DropIndex("dbo.FileChromaprintJobs", new[] { "File_FileID" });
            DropIndex("dbo.FileMD5Jobs", new[] { "File_FileID" });
            RenameColumn(table: "dbo.FileChromaprintJobs", name: "File_FileID", newName: "FileID");
            RenameColumn(table: "dbo.FileMD5Jobs", name: "File_FileID", newName: "FileID");
            AlterColumn("dbo.FileChromaprintJobs", "FileID", c => c.Int(nullable: false));
            AlterColumn("dbo.FileMD5Jobs", "FileID", c => c.Int(nullable: false));
            CreateIndex("dbo.FileChromaprintJobs", "FileID");
            CreateIndex("dbo.FileMD5Jobs", "FileID");
            AddForeignKey("dbo.FileChromaprintJobs", "FileID", "dbo.Files", "FileID", cascadeDelete: true);
            AddForeignKey("dbo.FileMD5Jobs", "FileID", "dbo.Files", "FileID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FileMD5Jobs", "FileID", "dbo.Files");
            DropForeignKey("dbo.FileChromaprintJobs", "FileID", "dbo.Files");
            DropIndex("dbo.FileMD5Jobs", new[] { "FileID" });
            DropIndex("dbo.FileChromaprintJobs", new[] { "FileID" });
            AlterColumn("dbo.FileMD5Jobs", "FileID", c => c.Int());
            AlterColumn("dbo.FileChromaprintJobs", "FileID", c => c.Int());
            RenameColumn(table: "dbo.FileMD5Jobs", name: "FileID", newName: "File_FileID");
            RenameColumn(table: "dbo.FileChromaprintJobs", name: "FileID", newName: "File_FileID");
            CreateIndex("dbo.FileMD5Jobs", "File_FileID");
            CreateIndex("dbo.FileChromaprintJobs", "File_FileID");
            AddForeignKey("dbo.FileMD5Jobs", "File_FileID", "dbo.Files", "FileID");
            AddForeignKey("dbo.FileChromaprintJobs", "File_FileID", "dbo.Files", "FileID");
        }
    }
}
