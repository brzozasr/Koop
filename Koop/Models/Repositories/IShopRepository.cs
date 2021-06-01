using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Koop.Extensions;
using Koop.models;
using Koop.Models.Auth;
using Koop.Models.RepositoryModels;
using Koop.Models.Util;
using Microsoft.AspNetCore.Http;

namespace Koop.Models.Repositories
{
    public enum OrderDirection
    {
        Asc,
        Desc
    }
    
    public interface IShopRepository
    {
        public IEnumerable<ProductsShop> GetProductsShop(Guid userId, Expression<Func<ProductsShop, object>> orderBy, int start,
            int count,
            OrderDirection orderDirection = OrderDirection.Asc, Guid categoryId = default(Guid), Guid productId = default(Guid));

        public IEnumerable<CooperatorOrder> GetCooperatorOrders(Guid cooperatorId, Guid orderId);
        public IEnumerable<CooperatorOrderFund> GetCooperatorOrdersFund(Guid cooperatorId, Guid orderId);
        
        IEnumerable<BasketsView> GetBaskets();
        IEnumerable<UserOrdersHistoryView> GetUserOrders(string firstName, string lastName);
        Supplier GetSupplier(Guid supplierId);
        public Product GetProductById(Guid productId);
        public ProblemResponse UpdateProduct(Product product, IFormFile picture);
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
        public ProblemResponse MakeOrder(Guid productId, Guid userId, int quantity);
        public ShopRepositoryReturn RemoveUserOrder(Guid orderedItemId);
        public ShopRepositoryReturn AddProduct(Product product);
        public IEnumerable<Product> GetProductsBySupplier(Guid supplierId);
        public void UpdateSupplier(Supplier supplier);
        public void ToggleSupplierAvailability(Supplier supplier);

        public void ToggleProductAvailability(Product product);
        
        public void ToggleSupplierBlocked(Supplier supplier);

        public void ToggleProductBlocked(Product product);
        
        public void ChangeOrderStatus(Order order, OrderStatuses status);

        public void ClearBaskets();

        public void AssignBaskets(Guid orderId);
        public IEnumerable<AvailableQuantity> GetAllAvailableQuantities(Guid productId);
        public ProblemResponse GetOrderedItemsCount(Guid userId);
        public ProblemResponse CheckProductAvailability(Guid productId);
        public ProblemResponse IsOrderOpen();
        public ProblemResponse CheckOrderStatus();
    }
}