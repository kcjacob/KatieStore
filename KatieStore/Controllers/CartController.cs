using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KatieStore.Models;

namespace KatieStore.Controllers
{
    public class CartController : Controller
    {
        protected KatieStoreEntities2 db = new KatieStoreEntities2();
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Cart
        public ActionResult Index()
        {
            return View(this.GetBasket(db));
        }

        // POST: Cart
        [HttpPost]
        public ActionResult Index(Basket model)
        {

            var basket = this.GetBasket(db);
            foreach (var updatedProduct in model.BasketProducts)
            {
                var product = basket.BasketProducts.FirstOrDefault(x => x.ProductID == updatedProduct.ProductID);
                if (product != null)
                {
                    product.Quantity = updatedProduct.Quantity;
                    product.Modified = DateTime.UtcNow;
                }
            }
            db.SaveChanges();
            db.BasketProducts.RemoveRange(basket.BasketProducts.Where(x => x.Quantity == 0));
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}