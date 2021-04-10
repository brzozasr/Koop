using System;
using System.Text.Json.Serialization;

namespace Koop.Models.RepositoryModels
{
    public class GrandeOrderItemReport
    {
        public Guid OrderedItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public decimal FundPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalFundPrice { get; set; }
        public int Quantity { get; set; }
        public string UnitName { get; set; }
        public Guid CoopId { get; set; }
        public string CoopFirstName { get; set; }
        public string CoopLastName { get; set; }
        public byte CoopFundValue { get; set; }
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAbbr { get; set; }
        [JsonIgnore]
        public GrandeOrderReport GrandeOrderReport { get; set; }
    }
}