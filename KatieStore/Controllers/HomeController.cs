using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KatieStore;

namespace KatieStore.Controllers
{
    public class HomeController : System.Web.Mvc.Controller
    {
        public ActionResult CartCount()
        {
            if (Request.Cookies.AllKeys.Contains("cart"))
            {
                HttpCookie cartCookie = Request.Cookies["cart"];
                var cookieValues = cartCookie.Value.Split(',');
                int quantity = int.Parse(cookieValues[1]);
                return Json(quantity, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Here's some updated text";

            return View();
        }
    }
}