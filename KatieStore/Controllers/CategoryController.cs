using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace KatieStore.Controllers
{
    public class CategoryController : Controller
    {
        KatieStore.Models.KatieStoreEntities2 db = new Models.KatieStoreEntities2();
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult Index(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return View(db.Categories.Find(id));
            }
            return View(new Models.Category
            {
                Category1 = db.Categories.Where(x => x.Category2 == null).ToList(),
                Products = db.Products.ToList(),
                ID = ""
            });
        }
    }
}