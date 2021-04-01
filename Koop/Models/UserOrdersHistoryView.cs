using System;
using System.Collections.Generic;

#nullable disable

namespace Koop.models
{
    public partial class UserOrdersHistoryView
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid? OrderId { get; set; }
        public DateTime? OrderStopDate { get; set; }
        public string Price { get; set; }
        public string OrderStatusName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
