using System;
using System.Collections.Generic;

namespace Koop.Models.RepositoryModels
{
    public class SupplierReport
    {
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAbbr { get; set; }
        public string Email { get; set; }
        public decimal TotalProfit { get; set; }
        public ICollection<SupplierReportOrder> SupplierReportOrder { get; set; }

        public SupplierReport()
        {
            SupplierReportOrder = new List<SupplierReportOrder>();
        }
    }
}