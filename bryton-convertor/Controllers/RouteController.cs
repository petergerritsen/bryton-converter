using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using bryton_convertor.Models;
using System.Xml.Linq;
using System.Globalization;

namespace bryton_convertor.Controllers
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
                var model = new Models.ViewModels.EditRouteViewModel();

                var context = new BrytonConvertorContext();

                var route = new Route() { Name = "Henk", Description = "Henk", Created = DateTime.Now };
                context.Routes.Add(route);

                // Parse XML
                XDocument doc = XDocument.Load(file.InputStream);
                XNamespace ns = XNamespace.Get("http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2");

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

                model.RouteId = route.RouteId;

                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult TrackPoints(int routeId)
        {
            var context = new Models.BrytonConvertorContext();

            var route = context.Routes.FirstOrDefault(x => x.RouteId == routeId);
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
                    Id = x.TrackPointId,
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
