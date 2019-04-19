using Dflat.Business.Models;
using System.Data.Entity;

namespace Dflat.EF6.DataAccess
{
    public class DataContext : DbContext
    {
        public DataContext() : base("Default")
        {
            // Fix to problem described at:
            // https://stackoverflow.com/questions/32607736/the-entity-framework-provider-type-system-data-entity-sqlserver-sqlproviderserv/32859171
            // and
            // https://stackoverflow.com/questions/14033193/entity-framework-provider-type-could-not-be-loaded
            var ensureDllIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }


        public DbSet<FileSourceFolder> FileSourceFolders { get; set; }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<FileSourceFolderScanJob> FileSourceFolderScanJobs { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExcludePath>().HasKey(table => new { table.ExcludePathID, table.FileSourceFolderID });
            modelBuilder.Entity<ExcludePath>().Property(table => table.ExcludePathID).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Job>().ToTable("Jobs");
            modelBuilder.Entity<FileSourceFolderScanJob>().ToTable("FileSourceFolderScanJobs");
        }

    }


    
}
