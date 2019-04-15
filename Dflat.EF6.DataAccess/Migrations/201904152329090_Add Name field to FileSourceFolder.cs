namespace Dflat.EF6.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNamefieldtoFileSourceFolder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileSourceFolders", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FileSourceFolders", "Name");
        }
    }
}
