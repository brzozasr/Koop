using System;
using System.Collections.Generic;

namespace Koop.Models.RepositoryModels
{
    public class SupplierProducts
    {
        public SupplierProducts()
        {
            SupplierProductsList = new List<SupplierProductsNode>();
        }
        
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAbbr { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Picture { get; set; }
        public DateTime? OrderClosingDate { get; set; }
        public double Receivables { get; set; }
        
        public virtual ICollection<SupplierProductsNode> SupplierProductsList { get; set; }
    }
}