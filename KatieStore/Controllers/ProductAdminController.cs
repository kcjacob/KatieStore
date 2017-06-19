using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KatieStore.Models;
using System.Net;
using System.Data.Entity;
using System.Data;

namespace KatieStore.Controllers
{
    [Authorize(Roles = "Administrator, ProductAdministrator")]
    public class ProductAdminController : Controller
    {
        private KatieStoreEntities2 db = new KatieStoreEntities2();

        // GET: ProductAdmin
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }

        // GET: ProductAdmin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: ProductAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductAdmin/Create
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Price,Description,Created,Modified")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: ProductAdmin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.Categories = db.Categories.Select(x => new SelectListItem { Text = x.ID, Value = x.ID });

            return View(product);
        }

        // POST: ProductAdmin/Edit/5
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, string[] CategoryNames, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var extension = System.IO.Path.GetExtension(image.FileName);
                var fileName = "/content/" + product.ID + extension;


                image.SaveAs(Server.MapPath(fileName));
                var dbProduct = db.Products.Find(product.ID);
                if (CategoryNames != null)
                {
                    foreach (string category in CategoryNames)
                    {
                        var cat = db.Categories.Find(category);
                        dbProduct.Categories.Add(cat);
                    }
                }
                dbProduct.ProductImages.Add(new ProductImage { URL = fileName, AlternateText = product.Description, Created = DateTime.UtcNow, Modified = DateTime.UtcNow });
                dbProduct.Name = product.Name;
                dbProduct.Price = product.Price;
                dbProduct.Description = product.Description;
                dbProduct.Modified = DateTime.UtcNow;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = db.Categories.Select(x => new SelectListItem { Text = x.ID, Value = x.ID });

            return View(product);
        }

        // GET: ProductAdmin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: ProductAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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