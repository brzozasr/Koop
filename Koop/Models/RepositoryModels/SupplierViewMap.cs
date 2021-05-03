using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Koop.Models.RepositoryModels
{
    public class SupplierViewMap
    {
        public Guid SupplierId { get; set; }
        public bool Blocked { get; set; }
        public bool Available { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAbbr { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Picture { get; set; }
        public DateTime? OrderClosingDate { get; set; }
        public Guid OproId { get; set; }

        public double Receivables { get; set; }
        public string OproFirstName { get; set; }
        public string OproLastName { get; set; }

        [NotMapped] private string _oproFullName;
        
        [NotMapped] public string OproFullName
        {
            get
            {
                return $"{OproFirstName} {OproLastName}";
            }
            set
            {
                OproFirstName = value.Split(" ")[0];
                OproLastName = value.Split(" ")[1];
            }
        }

        // [NotMapped] public string OproFullName => $"{OproFirstName} {OproLastName}";
    }
}