using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

#nullable disable

namespace Koop.Models
{
    public partial class User : IdentityUser<Guid>
    {
        public User()
        {
            Baskets = new HashSet<Basket>();
            Favorities = new HashSet<Favority>();
            OrderedItems = new HashSet<OrderedItem>();
            Suppliers = new HashSet<Supplier>();
            Works = new HashSet<Work>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Info { get; set; }
        public double? Debt { get; set; }
        public Guid? FundId { get; set; }
        public Guid? BasketId { get; set; }
        
        public virtual Fund Fund { get; set; }
        public virtual ICollection<Basket> Baskets { get; set; }
        public virtual ICollection<Favority> Favorities { get; set; }
        public virtual ICollection<OrderedItem> OrderedItems { get; set; }
        public virtual ICollection<Supplier> Suppliers { get; set; }
        public virtual ICollection<Work> Works { get; set; }
    }
}
