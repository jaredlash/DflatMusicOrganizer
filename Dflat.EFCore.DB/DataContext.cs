using Dflat.Data.EFCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace Dflat.Data.EFCore
{
    public class DataContext : DbContext
    {
        public DbSet<FileSourceFolder> FileSourceFolders { get; set; }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<FileSourceFolderScanJob> FileSourceFolderScanJobs { get; set; }


        public DataContext() { }


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
            modelBuilder.Entity<ExcludePath>().ToTable("ExcludePaths").HasKey(table => table.ExcludePathID);

            modelBuilder.Entity<ExcludePath>()
                .Property(table => table.ExcludePathID).ValueGeneratedOnAdd();
            modelBuilder.Entity<ExcludePath>()
                .Property(table => table.Path).IsRequired();

            modelBuilder.Entity<FileSourceFolder>().ToTable("FileSourceFolders").HasKey(table => table.FileSourceFolderID);
            modelBuilder.Entity<FileSourceFolder>().HasMany(p => p.ExcludePaths).WithOne().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FileSourceFolder>()
                .Property(table => table.Name).IsRequired();
            modelBuilder.Entity<FileSourceFolder>()
                .Property(table => table.Path).IsRequired();

            modelBuilder.Entity<Job>().ToTable("Jobs").HasKey(table => table.JobID);
            modelBuilder.Entity<Job>().Property(table => table.Description).IsRequired();
            modelBuilder.Entity<Job>().Property(table => table.Errors).IsRequired();
            modelBuilder.Entity<Job>().Property(table => table.CreationTime).IsRequired();
            modelBuilder.Entity<Job>().Property(table => table.Output).IsRequired();
            modelBuilder.Entity<Job>().HasDiscriminator<Application.Models.JobType>("JobType")
                .HasValue<FileSourceFolderScanJob>(Application.Models.JobType.FileSourceFolderScanJob);
        }
    }
}
