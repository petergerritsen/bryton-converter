using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Core.Model;
using System.IO;
using System.Xml.Schema;
using System.Reflection;
using System.Xml;
using System.Globalization;

namespace Core.XML
{
    public class Parser
    {
        private static bool ValidateTcx(XDocument doc) {
            TextReader schemaStream =
                new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Core.XML.XSD.TrainingCenterDatabasev2.xsd"));
            XmlSchemaSet schemaSet = new XmlSchemaSet() ;
            schemaSet.Add("http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2", XmlReader.Create(schemaStream));
            try
            {
                doc.Validate(schemaSet, null);
            }
            catch (Exception) {
                return false;
            }

            return true;
        }

        public static Core.Model.Route ParseTcx(Stream stream) {
            var context = new Core.Persistence.Context();

            // Parse XML
            XDocument doc = XDocument.Load(stream);
            if (!ValidateTcx(doc)){
                throw new ArgumentException("Invalid xml", "stream");
            }
            
            XNamespace ns = XNamespace.Get("http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2");

            var name = doc.Root.Elements(ns + "Courses").First().Descendants(ns + "Course").First().Descendants(ns + "Name").First().Value;

            var route = new Route() { Name = name, Description = "Bryton Convertor course", Created = DateTime.Now };
            context.Routes.Add(route);

            var coursePoints = new List<CoursePoint>();
            foreach (XElement coursePoint in doc.Descendants(ns + "CoursePoint"))
            {
                var point = new CoursePoint();
                point.Route = route;
                point.Latitude = decimal.Parse(coursePoint.Descendants(ns + "LatitudeDegrees").First().Value, CultureInfo.InvariantCulture);
                point.Longitude = decimal.Parse(coursePoint.Descendants(ns + "LongitudeDegrees").First().Value, CultureInfo.InvariantCulture);
                point.Name = coursePoint.Descendants(ns + "PointType").First().Value;
                coursePoints.Add(point);
                context.CoursePoints.Add(point);
            }

            foreach (XElement trackPoint in doc.Descendants(ns + "Trackpoint"))
            {
                var point = new TrackPoint();
                point.Route = route;
                point.Latitude = decimal.Parse(trackPoint.Descendants(ns + "LatitudeDegrees").First().Value, CultureInfo.InvariantCulture);
                point.Longitude = decimal.Parse(trackPoint.Descendants(ns + "LongitudeDegrees").First().Value, CultureInfo.InvariantCulture);
                point.Elevation = decimal.Parse(trackPoint.Descendants(ns + "AltitudeMeters").First().Value, CultureInfo.InvariantCulture);
                point.Distance = decimal.Parse(trackPoint.Descendants(ns + "DistanceMeters").First().Value, CultureInfo.InvariantCulture);

                point.CoursePoint = coursePoints.FirstOrDefault(x => x.Latitude == point.Latitude && x.Longitude == point.Longitude);

                context.TrackPoints.Add(point);
            }

            context.SaveChanges();

            return route;
        }
    }
}
