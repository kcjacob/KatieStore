using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KatieStore.Models;
using System.Net;
using System.Data.Entity;

namespace KatieStore.Controllers
{
    [Authorize(Roles = "Administrator, CategoryAdministrator, noob")]
    public class CategoryAdminController : Controller
    {
        private KatieStoreEntities2 db = new KatieStoreEntities2();

        // GET: CategoryAdmin
        public ActionResult Index()
        {
            var categories = db.Categories.Include(c => c.Category2);
            return View(categories.ToList());
        }

        // GET: CategoryAdmin/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: CategoryAdmin/Create
        public ActionResult Create()
        {
            ViewBag.ParentID = new SelectList(db.Categories, "ID", "ID");
            return View();
        }

        // POST: CategoryAdmin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ParentID,Created,Modified")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.Modified = DateTime.UtcNow;
                category.Created = DateTime.UtcNow;
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ParentID = new SelectList(db.Categories, "ID", "ID", category.ParentID);
            return View(category);
        }

        // GET: CategoryAdmin/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentID = new SelectList(db.Categories, "ID", "ID", category.ParentID);
            return View(category);
        }

        // POST: CategoryAdmin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ParentID,Created,Modified")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.Modified = DateTime.UtcNow;
                category.Created = DateTime.UtcNow;
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentID = new SelectList(db.Categories, "ID", "ParentID", category.ParentID);
            return View(category);
        }

        // GET: CategoryAdmin/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: CategoryAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
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