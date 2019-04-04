namespace Dflat.EF6.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLastScanEndDateInSourceFolders : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.FileSourceFolders", "LastScanEnd");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FileSourceFolders", "LastScanEnd", c => c.DateTime());
        }
    }
}
