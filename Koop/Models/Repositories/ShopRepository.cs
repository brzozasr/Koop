using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Koop.models;
using Koop.Models.RepositoryModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Koop.Models.Repositories
{
    public class ShopRepository : IShopRepository
    {
        private KoopDbContext _koopDbContext;
        private IMapper _mapper;

        public ShopRepository(KoopDbContext koopDbContext, IMapper mapper)
        {
            _koopDbContext = koopDbContext;
            _mapper = mapper;
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

        public Product GetProductById(Guid productId)
        {
            return _koopDbContext.Products.SingleOrDefault(p => p.ProductId == productId);
        }

        public void UpdateProduct(Product product)
        {
            var productExist = _koopDbContext.Products.SingleOrDefault(p => p.ProductId == product.ProductId);
            var productUpdated = _mapper.Map(product, productExist);
            
            _koopDbContext.Products.Update(productUpdated);
        }

        public void RemoveProduct(IEnumerable<Product> product)
        {
            _koopDbContext.Products.RemoveRange(product);
        }

        public IEnumerable<AvailableQuantity> GetAvailableQuantities(Guid productId)
        {
            return _koopDbContext.AvailableQuantities.Where(p => p.ProductId == productId);
        }

        public void UpdateAvailableQuantities(IEnumerable<AvailableQuantity>availableQuantity)
        {
            foreach (var item in availableQuantity)
            {
                var availableQuantityExist =
                    _koopDbContext.AvailableQuantities.SingleOrDefault(p =>
                        p.AvailableQuantityId == item.AvailableQuantityId);

                if (availableQuantityExist is not null)
                {
                    var availableQuantityUpdated = _mapper.Map(item, availableQuantityExist);

                    _koopDbContext.AvailableQuantities.Update(availableQuantityUpdated);
                }
                else
                {
                    AvailableQuantity availableQuantityNew = new AvailableQuantity();
                    var availableQuantityUpdated = _mapper.Map(item, availableQuantityNew);
                    _koopDbContext.AvailableQuantities.Add(availableQuantityUpdated);
                }
            }
        }

        public void RemoveAvailableQuantities(IEnumerable<AvailableQuantity> availableQuantity)
        {
            _koopDbContext.AvailableQuantities.RemoveRange(availableQuantity);
        }

        public IEnumerable<ProductCategoriesCombo> GetProductCategories(Guid productId)
        {
            List<ProductCategoriesCombo> productCategoriesLists = new List<ProductCategoriesCombo>();
            var categories = _koopDbContext.ProductCategories
                .Where(p => p.ProductId == productId)
                .Include(p => p.Category);

            foreach (var item in categories)
            {
                ProductCategoriesCombo productCategoriesCombo = new ProductCategoriesCombo();
                var productCategoriesComboPart1 = _mapper.Map(item, productCategoriesCombo);
                var productCategoriesComboPart2 = _mapper.Map(item.Category, productCategoriesComboPart1);
                productCategoriesLists.Add(productCategoriesComboPart2);
            }

            return productCategoriesLists;
        }
        
        public void UpdateProductCategories(IEnumerable<ProductCategoriesCombo> productCategoriesCombos)
        {
            foreach (var item in productCategoriesCombos)
            {
                var productCatExist =
                    _koopDbContext.ProductCategories.SingleOrDefault(p =>
                        p.ProductCategoryId == item.ProductCategoryId);

                if (productCatExist is not null)
                {
                    var productCatUpdated = _mapper.Map(item, productCatExist);

                    _koopDbContext.ProductCategories.Update(productCatUpdated);
                }
                else
                {
                    ProductCategory productCategoryNew = new ProductCategory();
                    var productCatUpdated = _mapper.Map(item, productCategoryNew);
                    _koopDbContext.ProductCategories.Add(productCatUpdated);
                }
            }
        }

        public void RemoveProductCategories(IEnumerable<ProductCategoriesCombo> productCategoriesCombos)
        {
            IEnumerable<ProductCategory> productCategories = new List<ProductCategory>();
            var productCategoriesToRemove = _mapper.Map<IEnumerable<ProductCategoriesCombo>, IEnumerable<ProductCategory>>(productCategoriesCombos);

            Console.WriteLine($"Len: {productCategoriesCombos.Count()}");
            foreach (var item in productCategoriesToRemove)
            {
                Console.WriteLine($"Item: {item.ProductCategoryId}");
            }
            
            _koopDbContext.ProductCategories.RemoveRange(productCategoriesToRemove);
        }

        public void UpdateCategories(IEnumerable<Category> productCategories)
        {
            foreach (var item in productCategories)
            {
                var categoriesExist =
                    _koopDbContext.Categories.SingleOrDefault(p =>
                        p.CategoryId == item.CategoryId);

                if (categoriesExist is not null)
                {
                    var categoriesUpdated = _mapper.Map(item, categoriesExist);

                    _koopDbContext.Categories.Update(categoriesUpdated);
                }
                else
                {
                    Category categoryNew = new Category();
                    var categoryUpdated = _mapper.Map(item, categoryNew);
                    _koopDbContext.Categories.Add(categoryUpdated);
                }
            }
        }

        public void RemoveCategories(IEnumerable<Category> productCategories)
        {
            _koopDbContext.Categories.RemoveRange(productCategories);
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
        
        public IEnumerable<BasketsView> GetBaskets()
        {
            return _koopDbContext.BasketViews.ToList();
        }

        public IEnumerable<UserOrdersHistoryView> GetUserOrders(string firstName, string lastName)
        {
            return _koopDbContext.UserOrdersHistoryViews.Where(c =>
                c.FirstName.ToLower() == firstName && c.LastName.ToLower() == lastName);
        }

        public SupplierView GetSupplier(string abbr)
        {
            // var supplier = _koopDbContext.SupplierViews
            //     .Include(s => s.Opro)
            //     // .Include(s=>s.Products)
            //     .Select(s=> new SupplierView()
            //     {
            //         SupplierId = s.SupplierId,
            //         SupplierName = s.SupplierName,
            //         SupplierAbbr = s.SupplierAbbr,
            //         Description = s.Description,
            //         Email = s.Email,
            //         Phone = s.Phone,
            //         Picture = s.Picture,
            //         OrderClosingDate = s.OrderClosingDate,
            //         OproId = s.OproId,
            //         OproFirstName = s.Opro.FirstName,
            //         OproLastName = s.Opro.LastName
            //     })
            //     .SingleOrDefault(s=>s.SupplierAbbr.ToLower() == abbr);

            // return supplier;
            
            return _koopDbContext.SupplierViews.SingleOrDefault(s=> s.SupplierAbbr.ToLower() == abbr);
        }

        public IEnumerable<Order> GetBigOrders()
        {
            throw new NotImplementedException();
        }
    }
}