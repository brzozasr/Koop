using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Koop.Models.RepositoryModels;
using Microsoft.EntityFrameworkCore;

namespace Koop.Models.Repositories
{
    public class ShopRepository : IShopRepository
    {
        private KoopDbContext _koopDbContext;

        public ShopRepository(KoopDbContext koopDbContext)
        {
            _koopDbContext = koopDbContext;
        }
        
        public IEnumerable<ProductsShop> GetProductsShop(Expression<Func<ProductsShop, object>> orderBy, int start, int count,
            OrderDirection orderDirection = OrderDirection.Asc)
        {
            /*var products = from pr in koopContext.Products
                    join pc in koopContext.ProductCategories on pr.ProductId equals pc.ProductId into
                        productCategoriesGroup
                    from pcg in productCategoriesGroup.DefaultIfEmpty()
                    join c in koopContext.Categories on pcg.CategoryId equals c.CategoryId into categoryGroup
                    //from cg in categoryGroup.DefaultIfEmpty()
                    join s in koopContext.Suppliers on pr.SupplierId equals s.SupplierId into suppliersGroup
                    from sg in suppliersGroup.DefaultIfEmpty()
                    join u in koopContext.Units on pr.UnitId equals u.UnitId into unitGroup
                    from ug in unitGroup.DefaultIfEmpty()
                    //group cg.CategoryName by new {pr, ug, sg} into catGroup
                    select new ProductsList()
                    {
                        ProductName = pr.ProductName,
                        Price = pr.Price,
                        Picture = pr.Picture,
                        Blocked = pr.Blocked,
                        Available = pr.Available,
                        Description = pr.Description,
                        Unit = ug.UnitName,
                        AmountMax = pr.AmountMax,
                        SupplierAbbr = sg.SupplierAbbr ?? "NaN",
                        Categories = String.Join(',', categoryGroup)  
                    };*/
            var products = _koopDbContext.Products
                .Include(p => p.Supplier)
                .Include(p => p.Unit)
                .Join(_koopDbContext.AvailableQuantities, product => product.ProductId, quantity => quantity.ProductId,
                    (product, quantity) => new {Product = product, Quantity = quantity})
                .Select(p => new ProductsShop()
                {
                    ProductName = p.Product.ProductName,
                    Price = p.Product.Price,
                    Picture = p.Product.Picture,
                    Blocked = p.Product.Blocked,
                    Available = p.Product.Available,
                    Description = p.Product.Description,
                    Unit = p.Product.Unit.UnitName,
                    AmountMax = p.Product.AmountMax,
                    SupplierAbbr = p.Product.Supplier.SupplierAbbr ?? "NaN",
                    CategoryNames = p.Product.ProductCategories.Select(p => p.Category.CategoryName),
                    Quantities = p.Product.AvailableQuantities.Select(p => p.Quantity)
                });
            
            var productsSorted = orderDirection == OrderDirection.Asc ? products.OrderBy(orderBy) : products.OrderByDescending(orderBy);
            var productsGrouped = productsSorted.Skip(start).Take(count).ToList().GroupBy(p => p.ProductName, p => p, (s, lists) => new {Key = s, Value = lists});

            List<ProductsShop> output = new List<ProductsShop>();
            foreach (var product in productsGrouped)
            {
                ProductsShop tmp = new ProductsShop()
                {
                    ProductName = product.Value.FirstOrDefault().ProductName,
                    Price = product.Value.FirstOrDefault().Price,
                    Picture = product.Value.FirstOrDefault().Picture,
                    Blocked = product.Value.FirstOrDefault().Blocked,
                    Available = product.Value.FirstOrDefault().Available,
                    Description = product.Value.FirstOrDefault().Description,
                    Unit = product.Value.FirstOrDefault().Unit,
                    AmountMax = product.Value.FirstOrDefault().AmountMax,
                    SupplierAbbr = product.Value.FirstOrDefault().SupplierAbbr ?? "NaN",
                    CategoryNames = product.Value.FirstOrDefault().CategoryNames,
                    Quantities = product.Value.FirstOrDefault().Quantities
                };
                output.Add(tmp);
            }
            
            return output;
        }
    }
}