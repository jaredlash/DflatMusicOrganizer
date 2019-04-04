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

    }


    
}
