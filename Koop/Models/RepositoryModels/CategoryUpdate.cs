using System;

namespace Koop.Models.RepositoryModels
{
    public class CategoryUpdate
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Picture { get; set; }
    }
}