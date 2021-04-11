using System;

namespace Koop.Models.RepositoryModels
{
    public class ProductsQuantityUpdate
    {
        public Guid ProductId { get; set; }
        public int AmountInMagazine { get; set; }
        public int? AmountMax { get; set; }
        
    }
}