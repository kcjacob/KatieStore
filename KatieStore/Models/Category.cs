//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KatieStore.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Category
    {
       
        public Category()
        {
            this.Category1 = new HashSet<Category>();
            this.Products = new HashSet<Product>();
        }
    
        public string ID { get; set; }
        public string ParentID { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
    
       
        public virtual ICollection<Category> Category1 { get; set; }
        public virtual Category Category2 { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
