using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bumper.Controllers
{
    public class DisasterController : Controller
    {
        private BumperEntities db = new BumperEntities();

        // GET: Disaster
        public ActionResult Index()
        {
            ViewBag.Machines = db.machine.ToList();
            return View();
        }

        // GET: Disaster/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Disaster/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Disaster/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Disaster/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Disaster/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Disaster/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Disaster/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
