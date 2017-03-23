using Bumper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bumper.Controllers
{
    public class AnalysisController : Controller
    {
        private BumperEntities db = new BumperEntities();

        // GET: Analysis
        public ActionResult Index()
        {
            ViewBag.Machines = db.machine.ToList();
            return View();
        }

        // POST: Analysis
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormAnalysis analysis)
        {
            if (ModelState.IsValid)
            {
                // ANALYZER !!!
            }
            ViewBag.Machines = db.machine.ToList();
            return View(db.incidence.ToList());
        }

        // GET: Analysis/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Analysis/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Analysis/Create
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

        // GET: Analysis/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Analysis/Edit/5
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

        // GET: Analysis/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Analysis/Delete/5
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
