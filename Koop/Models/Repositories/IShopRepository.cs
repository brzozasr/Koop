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
        IEnumerable<ProductsShop> GetProductsShop(Expression<Func<ProductsShop, object>> orderBy, int start, int count,
            OrderDirection orderDirection = OrderDirection.Asc);
        
        IEnumerable<Basket> GetBaskets();
        IEnumerable<UserOrdersHistoryView> GetUserOrders(string firstName, string lastName);
        Supplier GetSupplier(string abbr);
        IEnumerable<Order> GetBigOrders();
    }
}