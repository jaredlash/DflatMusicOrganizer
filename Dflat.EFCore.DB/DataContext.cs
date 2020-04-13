using Dflat.EFCore.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace Dflat.EFCore.DB
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DbSet<FileSourceFolderData> FileSourceFolders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = builder.Build();

            var connectionString = config.GetConnectionString("DflatMusicOrganizerDB");

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            buildFileSourceFolderModels(modelBuilder);

        }


        private void buildFileSourceFolderModels(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExcludePathData>().ToTable("ExcludePaths").HasKey(table => table.ExcludePathID);
            
            modelBuilder.Entity<ExcludePathData>()
                .Property(table => table.ExcludePathID).ValueGeneratedOnAdd();
            modelBuilder.Entity<ExcludePathData>()
                .Property(table => table.Path).IsRequired();

            modelBuilder.Entity<FileSourceFolderData>().ToTable("FileSourceFolders").HasKey(table => table.FileSourceFolderID);
            modelBuilder.Entity<FileSourceFolderData>().HasMany(p => p.ExcludePaths).WithOne().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FileSourceFolderData>()
                .Property(table => table.Name).IsRequired();
            modelBuilder.Entity<FileSourceFolderData>()
                .Property(table => table.Path).IsRequired();
        }
    }
}
