using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;


namespace Bumper.Functions
{
    public class HomeController : Controller
    {

        BumperEntities db = new BumperEntities();

        public ActionResult Index()
        {
            List<incidence> incs = db.incidence.Include(p => p.machine).ToList();
            ViewBag.Incidences = incs;
            return View();
        }

    }
}