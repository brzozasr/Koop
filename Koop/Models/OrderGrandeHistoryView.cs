using System;
using System.Collections.Generic;
using NodaTime;

#nullable disable

namespace Koop.models
{
    public partial class OrderGrandeHistoryView
    {
        public Guid? OrderId { get; set; }
        public DateTime? OrderStartDate { get; set; }
        public DateTime? OrderStopDate { get; set; }
        public Guid? OrderStatusId { get; set; }
        public string OrderStatusName { get; set; }
    }
}
