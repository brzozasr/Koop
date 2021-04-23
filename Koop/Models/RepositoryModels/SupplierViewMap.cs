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
        [NotMapped] public string OproFirstName { get; set; }
        [NotMapped] public string OproLastName { get; set; }

        [NotMapped] public string OproFullName => $"{OproFirstName} {OproLastName}";
    }
}