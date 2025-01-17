﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Koop.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            Products = new HashSet<Product>();
        }

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

        public virtual User Opro { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
