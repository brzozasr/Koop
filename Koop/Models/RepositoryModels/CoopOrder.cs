using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Koop.Models.RepositoryModels
{
    public class CoopOrder
    {
        
        public CoopOrder()
        {
            CoopOrderNode = new HashSet<CoopOrderNode>();
        }
        
        public Guid? OrderId { get; set; }
        public Guid? Id { get; set; }
        public DateTime? OrderStartDate { get; set; }
        public DateTime? OrderStopDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public short? FundValue { get; set; }
        [NotMapped] public decimal OrderTotalPrice { get; set; }
        [NotMapped] public decimal OrderTotalFundPrice { get; set; }
        public ICollection<CoopOrderNode> CoopOrderNode { get; set; }
    }
}