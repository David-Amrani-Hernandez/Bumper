using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bumper;

namespace Bumper.Controllers
{
    public class IncidencesController : Controller
    {
        private BumperEntities db = new BumperEntities();

        // GET: Incidences
        public ActionResult Index()
        {
            var incidence = db.incidence.Include(i => i.machine);
            return View(incidence.ToList());
        }

        // GET: Incidences/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            incidence incidence = db.incidence.Find(id);
            if (incidence == null)
            {
                return HttpNotFound();
            }
            return View(incidence);
        }

        // GET: Incidences/Create
        public ActionResult Create()
        {
            ViewBag.id_machine = new SelectList(db.machine, "id", "provider");
            return View();
        }

        // POST: Incidences/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,evidence,description,vulnerability,id_machine")] incidence incidence)
        {
            if (ModelState.IsValid)
            {
                db.incidence.Add(incidence);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_machine = new SelectList(db.machine, "id", "provider", incidence.id_machine);
            return View(incidence);
        }

        // GET: Incidences/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            incidence incidence = db.incidence.Find(id);
            if (incidence == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_machine = new SelectList(db.machine, "id", "provider", incidence.id_machine);
            return View(incidence);
        }

        // POST: Incidences/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,evidence,description,vulnerability,id_machine")] incidence incidence)
        {
            if (ModelState.IsValid)
            {
                db.Entry(incidence).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_machine = new SelectList(db.machine, "id", "provider", incidence.id_machine);
            return View(incidence);
        }

        // GET: Incidences/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            incidence incidence = db.incidence.Find(id);
            if (incidence == null)
            {
                return HttpNotFound();
            }
            return View(incidence);
        }

        // POST: Incidences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            incidence incidence = db.incidence.Find(id);
            db.incidence.Remove(incidence);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
