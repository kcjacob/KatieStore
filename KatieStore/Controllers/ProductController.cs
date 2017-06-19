using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KatieStore.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace KatieStore.Controllers
{
    public class ProductController : Controller
    {

        public ProductController()
        {
            
                db = new KatieStoreEntities2();
 
        }

        public ProductController(KatieStoreEntities2 entities = null, ControllerContext context = null)
        {
            if (entities == null)
            {
                db = new KatieStoreEntities2();
            }
            else
            {
                db = entities;
            }

            if (context != null)
            {
                ControllerContext = context;
            }
        }

        protected KatieStoreEntities2 db;
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Product
        [OutputCache(Duration = 30)]
        public async Task<ActionResult> Index(int? id)
        {

            Stopwatch timer = new Stopwatch();
            timer.Start();
            timer.Stop();
            ViewBag.LoadTime = timer.ElapsedMilliseconds + " milliseconds OR " + timer.ElapsedTicks + " ticks";
            return View(await db.Products.FindAsync(id));
        }


        [HttpPost]
        public async Task<ActionResult> Index(Product model, int? quantity)
        {
            Basket b = this.GetBasket(db);
            BasketProduct p = b.BasketProducts.FirstOrDefault(x => x.ProductID == model.ID);
            if (p != null)
            {
                p.Quantity += quantity ?? 1;
            }
            else
            {
                p = new BasketProduct
                {
                    ProductID = model.ID,
                    Quantity = quantity ?? 1,
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow
                };
                b.BasketProducts.Add(p);
            }
            b.Modified = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Cart");
        }

    }
}