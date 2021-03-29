using System;
using System.Collections.Generic;

#nullable disable

namespace Koop.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderedItems = new HashSet<OrderedItem>();
        }

        public Guid OrderId { get; set; }
        public DateTime OrderStartDate { get; set; }
        public DateTime OrderStopDate { get; set; }
        
        // public Guid OrderStatusId { get; set; }
        //
        // public virtual OrderStatus OrderStatus { get; set; }
        public virtual ICollection<OrderedItem> OrderedItems { get; set; }
    }
}
