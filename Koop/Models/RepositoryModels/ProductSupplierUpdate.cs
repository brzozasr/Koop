using System;

namespace Koop.Models.RepositoryModels
{
    public class ProductSupplierUpdate
    {
        public Guid ProductId { get; set; }
        public int? AmountMax { get; set; }
        public bool Available { get; set; }
        public bool Blocked { get; set; }
    }
}