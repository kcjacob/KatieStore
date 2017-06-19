using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KatieStore.Models
{
    public class ProductModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal? Price { get; set; }
    }
    public class ProductData
    {
        public static List<ProductModel> Products;
        static ProductData()
        {
            Products = new List<ProductModel>();
            Products.Add(new ProductModel { ID = 1, Description = "This cast-iron skillet provides even heat distribution and is oven safe", Name = "12\" iron skillet", Price = 32.99m });
            Products.Add(new ProductModel { ID = 2, Description = "The 10\" aluminum pan is lightweight and easy to clean", Name = "10\" Aluminum Pan", Price = 18.99m  });
        }

    }
}