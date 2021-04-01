using System;

namespace Koop.Models.RepositoryModels
{
    public class ProductCategoriesCombo
    {
        public Guid ProductCategoryId { get; set; }
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}