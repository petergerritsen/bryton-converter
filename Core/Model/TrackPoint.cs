using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.Model
{
    public class TrackPoint : Entity
    {
        public virtual Route Route { get; set; }

        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public decimal Elevation { get; set; }
        public decimal Distance { get; set; }

        public virtual TrackPointType Type { get; set; }
        public virtual CoursePoint CoursePoint { get; set; }
    }
}