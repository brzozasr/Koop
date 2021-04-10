using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Koop.Models.RepositoryModels
{
    public class SupplierReportOrder
    {
        public Guid OrderId { get; set; }
        public DateTime OrderStartDate { get; set; }
        public DateTime OrderStopDate { get; set; }
        public decimal OrderTotalPrice { get; set; }
        [JsonIgnore]
        public SupplierReport SupplierReport { get; set; }
        public List<SupplierReportItem> SupplierReportItems { get; set; }

        public SupplierReportOrder()
        {
            SupplierReportItems = new List<SupplierReportItem>();
        }
     }
}