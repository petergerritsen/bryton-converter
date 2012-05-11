using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bryton_convertor.Models
{
    public class CoursePoint
    {
        public int CoursePointId { get; set; }

        public virtual Route Route {get;set;}

        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string Name { get; set; }
    }
}