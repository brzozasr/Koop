using System;
using System.Collections.Generic;

#nullable disable

namespace Koop.Models
{
    public partial class Product
    {
        public Product()
        {
            AvailableQuantities = new HashSet<AvailableQuantity>();
            Favorities = new HashSet<Favority>();
            OrderedItems = new HashSet<OrderedItem>();
            ProductCategories = new HashSet<ProductCategory>();
        }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int AmountInMagazine { get; set; }
        public bool Magazine { get; set; }
        public int? AmountMax { get; set; }
        public int? Deposit { get; set; }
        public string Picture { get; set; }
        public Guid UnitId { get; set; }
        public Guid SupplierId { get; set; }
        public bool Available { get; set; }
        public bool Blocked { get; set; }

        public virtual Supplier Supplier { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual ICollection<AvailableQuantity> AvailableQuantities { get; set; }
        public virtual ICollection<Favority> Favorities { get; set; }
        public virtual ICollection<OrderedItem> OrderedItems { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
