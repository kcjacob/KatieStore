using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KatieStore.Models;

namespace KatieStore.Controllers
{
    public class KatieStoreController : Controller
    {
        protected KatieStoreEntities2 entities = new KatieStoreEntities2();
        protected override void Dispose(bool disposing)
        {
            entities.Dispose();
            base.Dispose(disposing);
        }

        protected Basket CurrentBasket
        {
            get
            {
                Basket b = null;
                if (User.Identity.IsAuthenticated)
                {
                    b = entities.AspNetUsers.FirstOrDefault(x => x.UserName == User.Identity.Name).Baskets.FirstOrDefault();
                }
                else if (Request.Cookies.AllKeys.Contains("cart"))
                {
                    int cartId = int.Parse(Request.Cookies["cart"].Value);
                    b = entities.Baskets.Find(cartId);
                }

                if (b == null)
                {
                    b = new Basket();
                    b.Created = DateTime.UtcNow;
                    b.Modified = DateTime.UtcNow;
                    if (User.Identity.IsAuthenticated)
                    {
                        b.AspNetUserID = entities.AspNetUsers.FirstOrDefault(x => x.UserName == User.Identity.Name).Id;
                    }
                    entities.Baskets.Add(b);
                    entities.SaveChanges();
                    if (!User.Identity.IsAuthenticated)
                    {
                        Response.Cookies.Add(new HttpCookie("cart", b.ID.ToString()));
                    }
                }

                return b;
            }
        }


    }
}