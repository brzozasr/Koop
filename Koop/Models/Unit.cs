﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Koop.Models
{
    public partial class Unit
    {
        public Unit()
        {
            Products = new HashSet<Product>();
        }

        public Guid UnitId { get; set; }
        public string UnitName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
