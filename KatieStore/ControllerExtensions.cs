using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KatieStore.Models;
using System.Web.Mvc;

namespace KatieStore
{
    public static class ControllerExtensions
    {
        public static Basket GetBasket(this Controller controller, KatieStoreEntities2 entities)
        {
            Basket b = null;
            if (controller.User.Identity.IsAuthenticated)
            {
                b = entities.AspNetUsers.FirstOrDefault(x => x.UserName == controller.User.Identity.Name).Baskets.FirstOrDefault();
            }
            else if (controller.HttpContext.Request.Cookies.AllKeys.Contains("cart"))
            {
                int cartId = int.Parse(controller.HttpContext.Request.Cookies["cart"].Value);
                b = entities.Baskets.Find(cartId);
            }

            if (b == null)
            {
                b = new Basket();
                b.Created = DateTime.UtcNow;
                b.Modified = DateTime.UtcNow;
                if (controller.HttpContext.User.Identity.IsAuthenticated)
                {
                    b.AspNetUserID = entities.AspNetUsers.FirstOrDefault(x => x.UserName == controller.HttpContext.User.Identity.Name).Id;
                }
                entities.Baskets.Add(b);
                entities.SaveChanges();
                if (!controller.HttpContext.User.Identity.IsAuthenticated)
                {
                    controller.HttpContext.Response.Cookies.Add(new HttpCookie("cart", b.ID.ToString()));
                }
            }

            return b;
        }
    }
}