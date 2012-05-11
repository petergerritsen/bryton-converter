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
                var context = new BrytonConvertorContext();

                var route = new Route() { Name = "Henk", Description = "Henk" };
                context.Routes.Add(route);
                
                // Parse XML
                XDocument doc = XDocument.Load(file.InputStream);
                XNamespace ns = XNamespace.Get("http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2");
                
                foreach(XElement trackPoint in doc.Descendants(ns + "Trackpoint")){
                    var point = new TrackPoint();
                    point.Route = route;
                    point.Latitude = decimal.Parse(trackPoint.Descendants(ns + "LatitudeDegrees").First().Value, CultureInfo.InvariantCulture);
                    point.Longitude = decimal.Parse(trackPoint.Descendants(ns + "LongitudeDegrees").First().Value, CultureInfo.InvariantCulture);
                    point.Elevation = decimal.Parse(trackPoint.Descendants(ns + "AltitudeMeters").First().Value, CultureInfo.InvariantCulture);
                    point.Distance = decimal.Parse(trackPoint.Descendants(ns + "DistanceMeters").First().Value, CultureInfo.InvariantCulture);
                    context.TrackPoints.Add(point);
                }


                context.SaveChanges();
            }
            else
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
