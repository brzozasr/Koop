using System;
using System.Collections.Generic;
using NodaTime;

#nullable disable

namespace Koop.models
{
    public partial class SupplierView
    {
        public Guid? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAbbr { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Picture { get; set; }
        public DateTime? OrderClosingDate { get; set; }
        public Guid OproId { get; set; }
        public string OproFirstName { get; set; }
        public string OproLastName { get; set; }

        public string OproFullName => $"{OproFirstName} {OproLastName}";
    }
}
