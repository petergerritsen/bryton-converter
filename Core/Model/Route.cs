using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.Model
{
    public class Route : Entity
    {        
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TrackPoint> TrackPoints { get; set; }
        public virtual ICollection<CoursePoint> CoursePoints { get; set; }
    }
}