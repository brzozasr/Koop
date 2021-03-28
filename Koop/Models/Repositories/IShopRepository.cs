using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    }
}