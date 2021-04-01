using System;
using System.Collections.Generic;

#nullable disable

namespace Koop.Models
{
    public partial class Fund
    {
        public Fund()
        {
            Cooperators = new HashSet<User>();
        }

        public Guid FundId { get; set; }
        public byte Value { get; set; }

        public virtual ICollection<User> Cooperators { get; set; }
    }
}
