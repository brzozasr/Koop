using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Koop.Extensions;
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
        public ShopRepositoryReturn UpdateProduct(Product product);
        public ShopRepositoryReturn RemoveProduct(IEnumerable<Product> product);
        public IEnumerable<ProductCategoriesCombo> GetProductCategories(Guid productId);
        public IEnumerable<AvailableQuantity> GetAvailableQuantities(Guid productId);
        public ShopRepositoryReturn UpdateAvailableQuantities(IEnumerable<AvailableQuantity> availableQuantity);
        public ShopRepositoryReturn RemoveAvailableQuantities(IEnumerable<AvailableQuantity> availableQuantity);
        public ShopRepositoryReturn UpdateProductCategories(IEnumerable<ProductCategoriesCombo> productCategoriesCombos);
        public ShopRepositoryReturn RemoveProductCategories(IEnumerable<ProductCategoriesCombo> productCategoriesCombos);
        public ShopRepositoryReturn UpdateCategories(IEnumerable<Category> productCategories);
        public ShopRepositoryReturn RemoveCategories(IEnumerable<Category> productCategories);
        public ShopRepositoryReturn UpdateUnits(IEnumerable<Unit> units);
        public ShopRepositoryReturn RemoveUnits(IEnumerable<Unit> units);
        public ShopRepositoryReturn UpdateUserOrderQuantity(Guid orderId, int quantity);
        public ShopRepositoryReturn UpdateUserOrderStatus(Guid orderId, Guid userId, Guid statusId);
        public ShopRepositoryReturn MakeOrder(Guid productId, Guid userId, int quantity);
        public ShopRepositoryReturn RemoveUserOrder(Guid orderedItemId);
        public ShopRepositoryReturn AddProduct(Product product);
    }
}