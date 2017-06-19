using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KatieStore.Models;

namespace KatieStore.Controllers
{
    public class ReceiptController : Controller
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
        // GET: Receipt
        public ActionResult Index(string id)
        {
            var purchase = db.Purchases.SingleOrDefault(x => x.OrderIdentifier == id);
            return View(purchase);
        }
    }
}