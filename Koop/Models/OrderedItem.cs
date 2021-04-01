using System;
using System.Collections.Generic;

#nullable disable

namespace Koop.Models
{
    public partial class OrderedItem
    {
        public Guid OrderedItemId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public Guid CoopId { get; set; }
        public Guid OrderStatusId { get; set; }
        public int Quantity { get; set; }

        public virtual User Coop { get; set; }
        public virtual Order Order { get; set; }
        public virtual OrderStatus OrderStatus { get; set; }
        public virtual Product Product { get; set; }
    }
}
