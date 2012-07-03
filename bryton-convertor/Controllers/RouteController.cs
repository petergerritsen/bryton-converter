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
using System.Threading;

namespace Web.Controllers
{
    public class RouteController : AsyncController
    {
        //
        // GET: /Route/

        public ActionResult Index()
        {
            return View();
        }

        [AsyncTimeout(300000)]
        public void UploadAsync(HttpPostedFileBase file)
        {
            if (file == null) {
                AsyncManager.Parameters["ModelIsValid"] = false;
            }

            if (file.ContentLength > 0)
            {
                

                AsyncManager.OutstandingOperations.Increment();
                new Thread(() =>
                {
                    var route = Core.XML.Parser.ParseTcx(file.InputStream);

                    AsyncManager.Parameters["ModelIsValid"] = true;
                    AsyncManager.Parameters["RouteId"] = route.Id;
                    AsyncManager.OutstandingOperations.Decrement();
                }).Start();
            }
            else
            {
                AsyncManager.Parameters["ModelIsValid"] = false; 
            }
        }

        public ActionResult UploadCompleted(HttpPostedFileBase file) {
            if ((bool)AsyncManager.Parameters["ModelIsValid"] == false)
            {
                return RedirectToAction("Index");
            }
            else {
                return RedirectToAction("Edit", new { routeId = (int)AsyncManager.Parameters["RouteId"] });
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
