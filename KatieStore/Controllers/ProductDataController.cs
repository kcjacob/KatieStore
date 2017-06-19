using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KatieStore.Controllers
{
    public class ProductDataController : ApiController
    {
        Models.KatieStoreEntities2 entities = new Models.KatieStoreEntities2();
        //Each method responds on a matching HTTP verb - calling "Get" on ProductData will return one of the GET methods:

        public Models.Product[] Get()
        {
            return entities.Products.ToArray();

        }


        //Most exciting part: WebAPI automatically handles the Serialization of data - either JSON or XML, based on the "Accept" header sent by the client
        public Models.Product Get(int id)
        {
            return entities.Products.FirstOrDefault(x => x.ID == id);
        }

        public IHttpActionResult Post(Models.Product model)
        {
            var p = entities.Products.FirstOrDefault(x => x.ID == model.ID);
            if (p != null)
            {
                //p. = model.Description;
                p.Name = model.Name;
                p.Price = model.Price;
                return this.Ok<Models.Product>(p);
            }
            return this.BadRequest("Product not found");
        }

        public IHttpActionResult Put(Models.Product model)
        {
            int nextId = entities.Products.Max(x => x.ID) + 1;
            model.ID = nextId;
            entities.Products.Add(model);

            return this.Created("/api/Get/" + nextId, model);
        }

        public IHttpActionResult Delete(int id)
        {
            entities.Products.Remove(entities.Products.Find(id));
            return this.Ok();
        }
    }
}