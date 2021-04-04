using System;

namespace Koop.Models.RepositoryModels
{
    public class CooperatorOrder
    {
        public Guid OrderedItemId { get; set; }
        public Guid ProductId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string OrderStatus { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double UnitPrice { get; set; }
    }
}