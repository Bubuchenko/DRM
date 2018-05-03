using DRM_Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Collections.Generic;
using System.IO;

namespace DRM_Data
{
    public class DRMContext : DbContext
    {
        public DRMContext(DbContextOptions<DRMContext> options) : base(options)
        {
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<Record> Records { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<Application>().HasMany(f => f.Tasks).WithOne().HasForeignKey(f => f.Application).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}