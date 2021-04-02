using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Koop.models;
using Koop.Models.RepositoryModels;

namespace Koop.Models.Repositories
{
    public enum OrderDirection
    {
        Asc,
        Desc
    }
    
    public interface IShopRepository
    {
        public IEnumerable<ProductsShop> GetProductsShop(Expression<Func<ProductsShop, object>> orderBy, int start,
            int count,
            OrderDirection orderDirection = OrderDirection.Asc, Guid productId = default(Guid));

        public IEnumerable<CooperatorOrder> GetCooperatorOrders(Guid cooperatorId, Guid orderId);
        
        IEnumerable<Basket> GetBaskets();
        IEnumerable<UserOrdersHistoryView> GetUserOrders(string firstName, string lastName);
        Supplier GetSupplier(string abbr);
        IEnumerable<Order> GetBigOrders();
        public Product GetProductById(Guid productId);
        public void UpdateProduct(Product product);
        public void RemoveProduct(IEnumerable<Product> product);
        public IEnumerable<ProductCategoriesCombo> GetProductCategories(Guid productId);
        public IEnumerable<AvailableQuantity> GetAvailableQuantities(Guid productId);
        public void UpdateAvailableQuantities(IEnumerable<AvailableQuantity> availableQuantity);
        public void RemoveAvailableQuantities(IEnumerable<AvailableQuantity> availableQuantity);
        public void UpdateProductCategories(IEnumerable<ProductCategoriesCombo> productCategoriesCombos);
        public void RemoveProductCategories(IEnumerable<ProductCategoriesCombo> productCategoriesCombos);
        public void UpdateCategories(IEnumerable<Category> productCategories);
        public void RemoveCategories(IEnumerable<Category> productCategories);
        public void UpdateUnits(IEnumerable<Unit> units);
    }
}