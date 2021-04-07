using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Koop.Models.RepositoryModels
{
    public class CoopOrderNode
    {
        public Guid? OrderId { get; set; }
        public Guid? OrderedItemId { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
        [JsonIgnore] public short? FundValue { get; set; }
        public double? Price { get; set; }
        [NotMapped] public decimal? FundPrice { get; set; }
        public int? Quantity { get; set; }
        [NotMapped] public decimal? TotalPrice { get; set; }
        [NotMapped] public decimal? TotalFundPrice { get; set; }
        public string OrderStatusName { get; set; }

        public decimal? CalculateFundPrice()
        {
            if (Price.HasValue && FundValue.HasValue)
            {
                var result = Price.Value + Price.Value * ((double) FundValue.Value / 100);
                return (decimal?) Math.Round(result, 2, MidpointRounding.AwayFromZero);
            }

            return null;
        }

        public decimal? CalculateTotalPrice()
        {
            if (Price.HasValue && Quantity.HasValue)
            {
                var result = Price.Value * Quantity.Value;
                return (decimal?) Math.Round(result, 2, MidpointRounding.AwayFromZero);
            }

            return null;
        }

        public decimal? CalculateTotalFundPrice()
        {
            if (Price.HasValue && FundValue.HasValue && Quantity.HasValue)
            {
                var result = (Price.Value + Price.Value * ((double) FundValue.Value / 100)) * Quantity.Value;
                return (decimal?) Math.Round(result, 2, MidpointRounding.AwayFromZero);
            }

            return null;
        }
    }
}