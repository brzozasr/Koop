using System;

namespace Koop.Models.RepositoryModels
{
    public class OrderedItemQuantityUpdate
    {
        public Guid OrderedItemId { get; set; }
        public int Quantity { get; set; }
    }
}