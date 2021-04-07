using System;

namespace Koop.Models.RepositoryModels
{
    public class DebtsToSuppliersReport
    {
        public Guid SupplierId { get; set; }           
        public string SupplierName { get; set; }       
        public string SupplierAbbr { get; set; }
        public string Email { get; set; }              
        public string Phone { get; set; }  
        public double Receivables { get; set; }
    }
}