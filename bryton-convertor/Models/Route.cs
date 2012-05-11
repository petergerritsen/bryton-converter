using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bryton_convertor.Models
{
    public class Route
    {
        public int RouteId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TrackPoint> TrackPoints { get; set; }
        public virtual ICollection<CoursePoint> CoursePoints { get; set; }
    }
}