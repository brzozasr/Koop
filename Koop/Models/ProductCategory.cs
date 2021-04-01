using System;
using System.Collections.Generic;

#nullable disable

namespace Koop.Models
{
    public partial class ProductCategory
    {
        public Guid ProductCategoryId { get; set; }
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Product Product { get; set; }
    }
}
