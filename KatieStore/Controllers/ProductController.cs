using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KatieStore.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
       /* public ActionResult Index(in? id)
        {
            if(id == 300)
            {
                return this.Redirect("/");
        
            }
            var product = new { id = id, name = "My Product", price = 299m, description = "This is a product" };
            return Json(product, JsonRequestBehavior.AllowGet);
        }*/

        [HttpPost]
        public ActionResult Index(int? id)
        {
            return RedirectToAction("Index", "Home");
        }
    }
}