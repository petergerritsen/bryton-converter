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
using System.Text;
using System.Web.Script.Serialization;

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

        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file == null) {
                return RedirectToAction("Index");
            }

            if (file.ContentLength > 0)
            {
                var route = Core.XML.Parser.ParseTcx(file.InputStream);

                return RedirectToAction("Edit", new { routeId = route.Id });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(int routeId) {
            var model = new Models.EditRouteViewModel();

            var context = new Context();

            var route = context.Routes.FirstOrDefault(x => x.Id == routeId);
            if (route == null)
                return new HttpStatusCodeResult(404);

            model.RouteId = route.Id;
            model.RouteName = route.Name;

            return View(model);
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
                Name = route.Name,
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
        
        public ActionResult Download(string model) {
            var ser = new JavaScriptSerializer();
            var newModel = ser.Deserialize<JsonViewModel>(model);
            
            var context = new Context();

            var route = context.Routes.FirstOrDefault(x => x.Id == newModel.routeId);
            if (route == null)
                return new HttpStatusCodeResult(404);

            route.Name = newModel.name;

            var doc = route.GetBdx(newModel.markers);                  
            
            return File(Encoding.UTF8.GetBytes(doc.ToString()), "text/xml", "route.bdx");                       
        }        
    }
}
