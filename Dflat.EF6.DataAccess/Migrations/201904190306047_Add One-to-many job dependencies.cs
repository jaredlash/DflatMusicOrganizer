namespace Dflat.EF6.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOnetomanyjobdependencies : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "DependentJobID", c => c.Int());
            AddColumn("dbo.Jobs", "DependentJob_JobID", c => c.Int());
            CreateIndex("dbo.Jobs", "DependentJob_JobID");
            AddForeignKey("dbo.Jobs", "DependentJob_JobID", "dbo.Jobs", "JobID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Jobs", "DependentJob_JobID", "dbo.Jobs");
            DropIndex("dbo.Jobs", new[] { "DependentJob_JobID" });
            DropColumn("dbo.Jobs", "DependentJob_JobID");
            DropColumn("dbo.Jobs", "DependentJobID");
        }
    }
}
