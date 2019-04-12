namespace Dflat.EF6.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileSourceFolders",
                c => new
                    {
                        FileSourceFolderID = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                        IncludeInScans = c.Boolean(nullable: false),
                        LastScanStart = c.DateTime(),
                    })
                .PrimaryKey(t => t.FileSourceFolderID);
            
            CreateTable(
                "dbo.ExcludePaths",
                c => new
                    {
                        ExcludePathID = c.Int(nullable: false, identity: true),
                        FileSourceFolderID = c.Int(nullable: false),
                        Path = c.String(),
                    })
                .PrimaryKey(t => new { t.ExcludePathID, t.FileSourceFolderID })
                .ForeignKey("dbo.FileSourceFolders", t => t.FileSourceFolderID, cascadeDelete: true)
                .Index(t => t.FileSourceFolderID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExcludePaths", "FileSourceFolderID", "dbo.FileSourceFolders");
            DropIndex("dbo.ExcludePaths", new[] { "FileSourceFolderID" });
            DropTable("dbo.ExcludePaths");
            DropTable("dbo.FileSourceFolders");
        }
    }
}
