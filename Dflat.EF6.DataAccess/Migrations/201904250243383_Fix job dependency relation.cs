namespace Dflat.EF6.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fixjobdependencyrelation : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Jobs", "DependentJobID");
            RenameColumn(table: "dbo.Jobs", name: "DependentJob_JobID", newName: "DependentJobID");
            RenameIndex(table: "dbo.Jobs", name: "IX_DependentJob_JobID", newName: "IX_DependentJobID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Jobs", name: "IX_DependentJobID", newName: "IX_DependentJob_JobID");
            RenameColumn(table: "dbo.Jobs", name: "DependentJobID", newName: "DependentJob_JobID");
            AddColumn("dbo.Jobs", "DependentJobID", c => c.Int());
        }
    }
}
