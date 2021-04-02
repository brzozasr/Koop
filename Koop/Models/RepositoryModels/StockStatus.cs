using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Koop.Models.RepositoryModels
{
    public class StockStatus : SupplierProductsNode
    {
        public Guid StockSupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAbbr { get; set; }
    }
}