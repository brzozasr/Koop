using System;
using System.Collections.Generic;

#nullable disable

namespace Koop.Models
{
    public partial class CoopOrderHistoryView
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid? OrderId { get; set; }
        public DateTime? OrderStopDate { get; set; }
        public string Price { get; set; }
        public string OrderStatusName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
