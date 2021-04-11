using System;
using System.Collections.Generic;

namespace Koop.Models.RepositoryModels
{
    public class GrandeOrderReport
    {
        public Guid OrderId { get; set; }
        public DateTime OrderStartDate { get; set; }
        public DateTime OrderStopDate { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalGrandePrice { get; set; }
        public decimal TotalGrandeFundPrice { get; set; }
        public ICollection<GrandeOrderItemReport> GrandeOrderItem { get; set; }

        public GrandeOrderReport()
        {
            GrandeOrderItem = new List<GrandeOrderItemReport>();
        }
    }
}