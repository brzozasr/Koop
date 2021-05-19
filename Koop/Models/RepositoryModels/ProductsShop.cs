using System;
using System.Collections.Generic;

namespace Koop.Models.RepositoryModels
{
    public class ProductsShop
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public string Picture { get; set; }
        public bool Blocked { get; set; }
        public string Unit { get; set; }
        public bool Available { get; set; }
        public int? AmountMax { get; set; }
        public string Description { get; set; }
        public string SupplierAbbr { get; set; }
        public IEnumerable<string> CategoryNames { get; set; }
        public IEnumerable<int> Quantities { get; set; }
        public bool Magazine { get; set; }
        public int? Deposit { get; set; }
        public bool Ordered { get; set; }
        public Guid ProductId { get; set; }
    }
}