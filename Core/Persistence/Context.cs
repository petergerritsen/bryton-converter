using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Core.Model;

namespace Core.Persistence
{
    public class Context : DbContext
    {
        public DbSet<Route> Routes { get; set; }
        public DbSet<TrackPoint> TrackPoints { get; set; }
        public DbSet<CoursePoint> CoursePoints { get; set; }
        public DbSet<TrackPointType> TrackPointTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrackPoint>().Property(x => x.Latitude).HasPrecision(18, 8);
            modelBuilder.Entity<TrackPoint>().Property(x => x.Longitude).HasPrecision(18, 8);
            modelBuilder.Entity<TrackPoint>().Property(x => x.Distance).HasPrecision(18, 8);
            modelBuilder.Entity<CoursePoint>().Property(x => x.Latitude).HasPrecision(18, 8);
            modelBuilder.Entity<CoursePoint>().Property(x => x.Longitude).HasPrecision(18, 8);

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Context, Configuration>());
        }
    }
}