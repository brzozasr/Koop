using System;
using System.Collections.Generic;
using NodaTime;

#nullable disable

namespace Koop.Models
{
    public partial class OrderView
    {
        public Guid? OrderId { get; set; }
        public Guid? OrderedItemId { get; set; }
        public DateTime? OrderStartDate { get; set; }
        public DateTime? OrderStopDate { get; set; }
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Info { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public double? Price { get; set; }
        public decimal? FundPrice { get; set; }
        public int? AmountInMagazine { get; set; }
        public bool? Available { get; set; }
        public bool? Blocked { get; set; }
        public string BasketName { get; set; }
        public int? Quantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TotalFundPrice { get; set; }
        public string OrderStatusName { get; set; }
    }
}
