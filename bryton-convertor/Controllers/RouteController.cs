using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Globalization;
using Core.Persistence;
using Core.Model;
using System.Xml;

namespace Web.Controllers
{
    public class RouteController : Controller
    {
        //
        // GET: /Route/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                var model = new Models.EditRouteViewModel();

                var context = new Core.Persistence.Context();
                               
                // Parse XML
                XDocument doc = XDocument.Load(file.InputStream);
                XNamespace ns = XNamespace.Get("http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2");
                                
                var name = doc.Root.Elements(ns + "Courses").First().Descendants(ns +"Course").First().Descendants(ns + "Name").First().Value;
                
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

                model.RouteId = route.Id;

                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult TrackPoints(int routeId)
        {
            var context = new Context();

            var route = context.Routes.FirstOrDefault(x => x.Id == routeId);
            if (route == null)
                return new HttpStatusCodeResult(404);

            var data = new
            {
                Id = routeId,
                MaxLong = route.TrackPoints.Max(x => x.Longitude),
                MinLong = route.TrackPoints.Min(x => x.Longitude),
                MaxLat = route.TrackPoints.Max(x => x.Latitude),
                MinLat = route.TrackPoints.Min(x => x.Latitude),
                Distance = route.TrackPoints.Max(x => x.Distance),
                Points = route.TrackPoints.Select(x => new
                {
                    Id = x.Id,
                    Lat = x.Latitude,
                    Long = x.Longitude,
                    Ele = x.Elevation,
                    Dist = x.Distance
                }).OrderBy(x => x.Dist).ToArray()
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
