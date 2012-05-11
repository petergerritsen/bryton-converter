using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace bryton_convertor.Models
{
    public class BrytonConvertorContext : DbContext
    {
        public DbSet<Route> Routes { get; set; }
        public DbSet<TrackPoint> TrackPoints { get; set; }
        public DbSet<CoursePoint> CoursePoints { get; set; }
        public DbSet<TrackPointType> TrackPointTypes { get; set; }
    }
}