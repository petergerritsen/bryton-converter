using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Core.Model
{
    public class Route : Entity
    {        
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TrackPoint> TrackPoints { get; set; }
        public virtual ICollection<CoursePoint> CoursePoints { get; set; }

        public XDocument GetBdx(MarkerJsonModel[] markers) {

            var startPoint = TrackPoints.First();
            var endPoint = TrackPoints.Last();

            var trackPoints = new List<XElement>();
            foreach (var point in TrackPoints)
            {
                var trkPoint = new XElement("trkpt", 
                    new XAttribute("lat", point.Latitude),
                    new XAttribute("lon", point.Longitude),
                    new XElement("ele", point.Elevation)                    
                );
                
                var marker = markers.FirstOrDefault(x => x.pointId == point.Id);
                if (marker != null)
                {
                    if (marker.pointType == "Valley")
                        trkPoint.Add(new XElement("type", new XText("8")));
                    if (marker.pointType == "Peak")
                        trkPoint.Add(new XElement("type", new XText("4")));                        
                }

                trackPoints.Add(trkPoint);
            }

            XDocument doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement("gpx",                    
                    new XAttribute("creator", "bryton"),
                    new XAttribute("version", "2.2.0.3"),
                    new XAttribute("schemaLocation",  "http://www.brytonsport.com/BDX/2/2 http://www.brytonsport.com/BDX/2/2/BDX.xsd"),
                    //new XAttribute("xmlns", "http://www.topografix.com/GPX/1/1"),
                    new XElement("metadata",
                        new XElement("name", new XText(Name))
                    ),
                    new XElement("rte",
                        new XElement("name", new XText(Name)),
                        new XElement("number", new XText("2")),
                        new XElement("type", 
                            new XAttribute("isStopWatch", "false"),
                            new XText("0")
                        ),
                        new XElement("rtept", 
                            new XAttribute("lat", startPoint.Latitude),
                            new XAttribute("lon", startPoint.Longitude),
                            new XElement("type", new XText("1"))
                        ),
                        new XElement("rtept", 
                            new XAttribute("lat", endPoint.Latitude),
                            new XAttribute("lon", endPoint.Longitude),
                            new XElement("type", new XText("2"))
                        )
                    ),
                    new XElement("trk",
                        new XElement("trkseg",
                            trackPoints.ToArray())
                    )
                )
            );
            
            return doc;
        }
    }
}