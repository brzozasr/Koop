using System;

namespace Koop.Models.RepositoryModels
{
    public class CoopOrder
    {
        public Guid? OrderId { get; set; }
        public Guid? OrderedItemId { get; set; }
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProductName { get; set; }
        public double? Price { get; set; }
        public decimal? FundPrice { get; set; }
        public int? Quantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TotalFundPrice { get; set; }
        public string OrderStatusName { get; set; }
    }
}