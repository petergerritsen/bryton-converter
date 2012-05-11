using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using bryton_convertor.Models;

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

                // Parse XML

                context.Routes.Add(route);

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
