using System;
using System.Collections.Generic;

#nullable disable

namespace Koop.Models
{
    public partial class AvailableQuantity
    {
        public Guid AvailableQuantityId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        public virtual Product Product { get; set; }
    }
}
