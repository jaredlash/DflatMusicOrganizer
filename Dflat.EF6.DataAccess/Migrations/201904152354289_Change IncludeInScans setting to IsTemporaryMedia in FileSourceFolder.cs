namespace Dflat.EF6.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeIncludeInScanssettingtoIsTemporaryMediainFileSourceFolder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileSourceFolders", "IsTemporaryMedia", c => c.Boolean(nullable: false));
            DropColumn("dbo.FileSourceFolders", "IncludeInScans");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FileSourceFolders", "IncludeInScans", c => c.Boolean(nullable: false));
            DropColumn("dbo.FileSourceFolders", "IsTemporaryMedia");
        }
    }
}
