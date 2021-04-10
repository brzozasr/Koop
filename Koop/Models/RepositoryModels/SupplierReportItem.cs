using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Koop.Models.RepositoryModels
{
    public class SupplierReportItem
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } 
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string UnitName { get; set; }
        public decimal TotalPrice { get; set; }
        [JsonIgnore]
        public SupplierReportOrder SupplierReportOrder { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is SupplierReportItem product)
            {
                return ProductId == product.ProductId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ProductId.GetHashCode();
        }
    }
}