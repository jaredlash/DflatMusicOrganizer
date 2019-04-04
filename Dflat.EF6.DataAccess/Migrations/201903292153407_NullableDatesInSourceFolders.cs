namespace Dflat.EF6.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableDatesInSourceFolders : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FileSourceFolders", "LastScanStart", c => c.DateTime());
            AlterColumn("dbo.FileSourceFolders", "LastScanEnd", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FileSourceFolders", "LastScanEnd", c => c.DateTime(nullable: false));
            AlterColumn("dbo.FileSourceFolders", "LastScanStart", c => c.DateTime(nullable: false));
        }
    }
}
