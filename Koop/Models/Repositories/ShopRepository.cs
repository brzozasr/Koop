using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using Koop.models;
using Koop.Models.RepositoryModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

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
            OrderDirection orderDirection = OrderDirection.Asc, Guid productId = default(Guid))
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
            var products_tmp = productId == Guid.Empty
                ? _koopDbContext.Products
                : _koopDbContext.Products.Where(p => p.ProductId == productId);
            
            var products = products_tmp
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
                    Quantities = p.Product.AvailableQuantities.Select(p => p.Quantity),
                    Magazine = p.Product.Magazine,
                    Deposit = p.Product.Deposit
                });
            
            var productsSorted = orderDirection == OrderDirection.Asc ? products.OrderBy(orderBy) : products.OrderByDescending(orderBy);
            var productsGrouped = productsSorted.Skip(start).Take(count).ToList().GroupBy(p => p.ProductName, p => p, (s, lists) => new {Key = s, Value = lists});

            List<ProductsShop> output = new List<ProductsShop>();
            foreach (var product in productsGrouped)
            {
                var data = product.Value.FirstOrDefault();
                
                ProductsShop tmp = new ProductsShop()
                {
                    ProductName = data.ProductName,
                    Price = data.Price,
                    Picture = data.Picture,
                    Blocked = data.Blocked,
                    Available = data.Available,
                    Description = data.Description,
                    Unit = data.Unit,
                    AmountMax = data.AmountMax,
                    SupplierAbbr = data.SupplierAbbr ?? "NaN",
                    CategoryNames = data.CategoryNames,
                    Quantities = data.Quantities,
                    Magazine = data.Magazine,
                    Deposit = data.Deposit
                };
                
                output.Add(tmp);
            }
            
            return output;
        }

        public ProductsShop GetProductById(Guid productId)
        {
            return GetProductsShop(p => p.ProductName, 0, 1, productId: productId).SingleOrDefault();
        }

        public IEnumerable<CooperatorOrder> GetCooperatorOrders(Guid cooperatorId, Guid orderId)
        {
            var order = _koopDbContext.OrderedItems
                .Include(p => p.OrderStatus)
                .Include(p => p.Product)
                .ThenInclude(p => p.Unit)
                .Join(_koopDbContext.Users, item => item.CoopId, user => user.Id,
                    (item, user) => new {Products = item, User = user})
                .Where(p => p.User.Id == cooperatorId)
                .Where(p => p.Products.OrderId == orderId);

            List<CooperatorOrder> output = new List<CooperatorOrder>();
            foreach (var item in order)
            {
                CooperatorOrder cooperatorOrder = new CooperatorOrder()
                {
                    FirstName = item.User.FirstName,
                    LastName = item.User.LastName,
                    ProductName = item.Products.Product.ProductName,
                    Unit = item.Products.Product.Unit.UnitName,
                    OrderStatus = item.Products.OrderStatus.OrderStatusName,
                    Quantity = item.Products.Quantity,
                    Price = item.Products.Product.Price * item.Products.Quantity,
                    UnitPrice = item.Products.Product.Price
                };
                
                output.Add(cooperatorOrder);
            }

            return output;
        }
        
        public IEnumerable<Basket> GetBaskets()
        {
            var baskets = _koopDbContext.Baskets
                .Include(b => b.Coop)
                .Where(b=>b.CoopId != null)
                .Select(b => new Basket()
                {
                    BasketId = b.BasketId,
                    BasketName = b.BasketName,
                    CoopName = $"{b.Coop.FirstName} {b.Coop.LastName}",
                    CoopId = b.CoopId
                });
            
            return baskets;
        }

        public IEnumerable<UserOrdersHistoryView> GetUserOrders(string firstName, string lastName)
        {
            return _koopDbContext.UserOrdersHistoryView.Where(c =>
                c.FirstName.ToLower() == firstName && c.LastName.ToLower() == lastName);
        }

        public Supplier GetSupplier(string abbr)
        {
            var supplier = _koopDbContext.Suppliers
                .Include(s => s.Opro)
                // .Include(s=>s.Products)
                .Select(s=> new Supplier()
                {
                    SupplierId = s.SupplierId,
                    SupplierName = s.SupplierName,
                    SupplierAbbr = s.SupplierAbbr,
                    Description = s.Description,
                    Email = s.Email,
                    Phone = s.Phone,
                    Picture = s.Picture,
                    OrderClosingDate = s.OrderClosingDate,
                    OproId = s.OproId,
                    OproName = $"{s.Opro.FirstName} {s.Opro.LastName}",
                })
                .SingleOrDefault(s=>s.SupplierAbbr.ToLower() == abbr);
            
            return supplier;
        }

        public IEnumerable<Order> GetBigOrders()
        {
            throw new NotImplementedException();
        }
    }
}