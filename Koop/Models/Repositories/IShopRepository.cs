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
        public ProductsShop GetProductById(Guid productId);
    }
}