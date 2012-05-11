using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bryton_convertor.Models
{
    public class TrackPoint
    {
        public int TrackPointId { get; set; }

        public virtual Route Route { get; set; }

        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public decimal Elevation { get; set; }
        public decimal Distance { get; set; }

        public virtual TrackPointType Type { get; set; }
        public virtual CoursePoint CoursePoint { get; set; }
    }
}