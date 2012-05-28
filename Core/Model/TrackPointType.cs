using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.Model
{
    public class TrackPointType : Entity
    {        
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<TrackPoint> TrackPoints { get; set; }
    }
}