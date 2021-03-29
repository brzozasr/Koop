using System;

#nullable disable

namespace Koop.Models
{
    public partial class OrderView
    {
        public Guid? OrderId { get; set; }
        public Guid? OrderedItemId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProductName { get; set; }
        public DateTime? OrderStartDate { get; set; }
        public DateTime? OrderStopDate { get; set; }
        public string BasketName { get; set; }
        public int? Quantity { get; set; }
        public string OrderStatusName { get; set; }
    }
}
