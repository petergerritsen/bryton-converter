using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bryton_convertor.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var context = new Models.BrytonConvertorContext();
            var types = context.TrackPointTypes.Select(x => x.Name).ToList();

            ViewData.Add("Test", string.Join(", ", types));

            return View();
        }

    }
}
