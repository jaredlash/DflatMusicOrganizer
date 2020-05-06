﻿// <auto-generated />
using System;
using Dflat.Application.Models;
using Dflat.Data.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dflat.EFCore.DB.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20200503142508_Cancelled to JobStatuses")]
    partial class CancelledtoJobStatuses
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Dflat.EFCore.DB.Models.ExcludePath", b =>
                {
                    b.Property<int>("ExcludePathID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("FileSourceFolderID")
                        .HasColumnType("int");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ExcludePathID");

                    b.HasIndex("FileSourceFolderID");

                    b.ToTable("ExcludePaths");
                });

            modelBuilder.Entity("Dflat.EFCore.DB.Models.FileSourceFolder", b =>
                {
                    b.Property<int>("FileSourceFolderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsTemporaryMedia")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastScanStart")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FileSourceFolderID");

                    b.ToTable("FileSourceFolders");
                });

            modelBuilder.Entity("Dflat.EFCore.DB.Models.Job", b =>
                {
                    b.Property<int>("JobID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Errors")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IgnoreCache")
                        .HasColumnType("bit");

                    b.Property<int>("JobType")
                        .HasColumnType("int");

                    b.Property<string>("Output")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("JobID");

                    b.ToTable("Jobs");

                    b.HasDiscriminator<int>("JobType");
                });

            modelBuilder.Entity("Dflat.EFCore.DB.Models.FileSourceFolderScanJob", b =>
                {
                    b.HasBaseType("Dflat.EFCore.DB.Models.Job");

                    b.Property<int>("FileSourceFolderID")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("Dflat.EFCore.DB.Models.ExcludePath", b =>
                {
                    b.HasOne("Dflat.EFCore.DB.Models.FileSourceFolder", null)
                        .WithMany("ExcludePaths")
                        .HasForeignKey("FileSourceFolderID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
