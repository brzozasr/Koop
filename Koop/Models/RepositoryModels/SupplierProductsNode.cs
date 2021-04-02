using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Koop.Models.RepositoryModels
{
    public class SupplierProductsNode
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        [NotMapped]
        public string CategoryName { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int AmountInMagazine { get; set; }
        public bool Magazine { get; set; }
        public int? AmountMax { get; set; }
        public int? Deposit { get; set; }
        public string Picture { get; set; }
        [JsonIgnore] 
        public Guid UnitId { get; set; }
        [NotMapped]
        public string UnitName { get; set; }
        public bool Available { get; set; }
        public bool Blocked { get; set; }
    }
}