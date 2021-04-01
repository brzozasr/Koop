using System;
using System.Collections.Generic;

#nullable disable

namespace Koop.Models
{
    public partial class Work
    {
        public Guid WorkId { get; set; }
        public DateTime WorkDate { get; set; }
        public double Duration { get; set; }
        public Guid CoopId { get; set; }
        public Guid WorkTypeId { get; set; }

        public virtual User Coop { get; set; }
        public virtual WorkType WorkType { get; set; }
    }
}
