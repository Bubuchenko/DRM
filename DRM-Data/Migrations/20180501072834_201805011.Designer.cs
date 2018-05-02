﻿// <auto-generated />
using DRM_Data;
using DRM_Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace DRMData.Migrations
{
    [DbContext(typeof(DRMContext))]
    [Migration("20180501072834_201805011")]
    partial class _201805011
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DRM_Data.Application", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("DRM_Data.Configuration", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Database");

                    b.Property<string>("Logon");

                    b.Property<string>("Password");

                    b.Property<string>("Server");

                    b.HasKey("ID");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("DRM_Data.Models.Condition", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Selector");

                    b.Property<int>("Type");

                    b.Property<string>("Value");

                    b.HasKey("ID");

                    b.ToTable("Conditions");
                });

            modelBuilder.Entity("DRM_Data.Task", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ApplicationID");

                    b.Property<string>("ColumnName");

                    b.Property<int?>("ConditionID");

                    b.Property<int?>("ConfigurationID");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<string>("TableName");

                    b.Property<int>("Type");

                    b.HasKey("ID");

                    b.HasIndex("ApplicationID");

                    b.HasIndex("ConditionID");

                    b.HasIndex("ConfigurationID");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("DRM_Data.Task", b =>
                {
                    b.HasOne("DRM_Data.Application", "Application")
                        .WithMany("Tasks")
                        .HasForeignKey("ApplicationID");

                    b.HasOne("DRM_Data.Models.Condition", "Condition")
                        .WithMany()
                        .HasForeignKey("ConditionID");

                    b.HasOne("DRM_Data.Configuration", "Configuration")
                        .WithMany()
                        .HasForeignKey("ConfigurationID");
                });
#pragma warning restore 612, 618
        }
    }
}
