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
    
    public partial class Basket
    {
       
        public Basket()
        {
            this.BasketProducts = new HashSet<BasketProduct>();
        }
    
        public int ID { get; set; }
        public string AspNetUserID { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        
        public virtual ICollection<BasketProduct> BasketProducts { get; set; }
    }
}
