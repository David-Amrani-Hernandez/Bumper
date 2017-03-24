using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bumper.Functions
{
    public class HomeController : Controller
    {

        BumperEntities db = new BumperEntities();

        public ActionResult Index()
        {
            ViewBag.Incidences = db.incidence.ToList();
            return View();
        }

    }
}