using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model
{

    public class JsonViewModel
    {
        public int routeId { get; set; }
        public string name { get; set; }
        public MarkerJsonModel[] markers { get; set; }
    }

    public class MarkerJsonModel
    {
        public int pointId { get; set; }
        public string pointType { get; set; }
    }

}
