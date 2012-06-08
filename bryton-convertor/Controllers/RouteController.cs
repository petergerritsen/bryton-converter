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
            if (file == null) {
                return RedirectToAction("Index");
            }

            if (file.ContentLength > 0)
            {
                var model = new Models.EditRouteViewModel();
                
                var route = Core.XML.Parser.ParseTcx(file.InputStream);

                model.RouteId = route.Id;
                model.RouteName = route.Name;

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
