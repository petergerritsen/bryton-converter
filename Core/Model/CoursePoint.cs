using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.Model
{
    public class CoursePoint : Entity
    {
        public virtual Route Route {get;set;}

        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string Name { get; set; }
    }
}