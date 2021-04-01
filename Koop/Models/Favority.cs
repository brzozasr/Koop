using System;
using System.Collections.Generic;

#nullable disable

namespace Koop.Models
{
    public partial class Favority
    {
        public Guid FavoriteId { get; set; }
        public Guid CoopId { get; set; }
        public Guid ProductId { get; set; }

        public virtual User Coop { get; set; }
        public virtual Product Product { get; set; }
    }
}
