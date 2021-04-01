using System;
using System.Collections.Generic;

#nullable disable

namespace Koop.Models
{
    public partial class WorkType
    {
        public WorkType()
        {
            Works = new HashSet<Work>();
        }

        public Guid WorkTypeId { get; set; }
        public string WorkType1 { get; set; }

        public virtual ICollection<Work> Works { get; set; }
    }
}
