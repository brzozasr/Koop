using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;
using AutoMapper;
using Koop.Extensions;
using Koop.models;
using Koop.Models.Auth;
using Koop.Models.RepositoryModels;
using Koop.Models.Util;
using Microsoft.AspNetCore.Http;
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
        
        public IEnumerable<ProductsShop> GetProductsShop(Guid userId, Expression<Func<ProductsShop, object>> orderBy, int start, int count,
            OrderDirection orderDirection = OrderDirection.Asc, Guid categoryId = default(Guid), Guid productId = default(Guid))
        {
            IQueryable<Guid> productsIdOfCategory = null;
            if (categoryId != default(Guid))
            {
                productsIdOfCategory = _koopDbContext.ProductCategories
                    .Where(p => p.CategoryId == categoryId)
                    .Select(p => p.ProductId);
            }

            var activeOrderId = _koopDbContext.Orders.SingleOrDefault(p => p.OrderStatus.OrderStatusName == "Szkic")?.OrderId ?? Guid.Empty;
            
            var orderedProducts = _koopDbContext.OrderedItems
                .Where(p => p.CoopId == userId && p.OrderId == activeOrderId)
                .Select(p => p.ProductId);
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
            
            if (categoryId !=default(Guid))
            {
                products_tmp = products_tmp.Where(p => productsIdOfCategory.Any(sp => sp == p.ProductId));
            }

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
                    Deposit = p.Product.Deposit,
                    Ordered = orderedProducts.Any(o => o == p.Product.ProductId)
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
                    Deposit = data.Deposit,
                    Ordered = data.Ordered
                };
                
                output.Add(tmp);
            }
            
            return output;
        }

        public Product GetProductById(Guid productId)
        {
            return _koopDbContext.Products.SingleOrDefault(p => p.ProductId == productId);
        }

        public ShopRepositoryReturn AddProduct(Product product)
        {
            Product newProductTmp = new Product();
            var newProduct = _mapper.Map(product, newProductTmp);
            _koopDbContext.Products.Add(newProduct);

            return ShopRepositoryReturn.AddProductSuccess;
        }

        public ProblemResponse UpdateProduct(Product product, IFormFile picture)
        {
            ProblemResponse problemResponse = new ProblemResponse()
            {
                Detail = "Wystąpił błąd nieznanego pochodzenia",
                Status = 500
            };
            
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                Product updatedProduct;
                if (product.ProductId == Guid.Empty)
                {
                    Product newProductTmp = new Product();
                    var newProduct = _mapper.Map(product, newProductTmp);
                    _koopDbContext.Products.Add(newProduct);
                    updatedProduct = newProduct;
                }
                else
                {
                    var productExist = _koopDbContext.Products.SingleOrDefault(p => p.ProductId == product.ProductId);

                    if (productExist is null)
                    {
                        throw new Exception($"Nie znaleziono produktu o podanym Id: {product.ProductId}");
                    }
                
                    var productUpdated = _mapper.Map(product, productExist);
                    _koopDbContext.Products.Update(productUpdated);
                    updatedProduct = productUpdated;
                }
                _koopDbContext.SaveChanges();

                Console.WriteLine($"productID: {updatedProduct.ProductId}");

                if (updatedProduct.Category.Any())
                {
                    var currentProductCategories =
                        _koopDbContext.ProductCategories.Where(p => p.ProductId == product.ProductId).Include(p => p.Category);
                    
                    var categoriesToAdd = updatedProduct.Category.Where(p => !currentProductCategories.Select(q => q.Category.CategoryName).Contains(p.CategoryName));
                    var categoriesToRemove = currentProductCategories.Where(p =>
                        !updatedProduct.Category.Select(q => q.CategoryName).Contains(p.Category.CategoryName));
                    
                    foreach (var category in categoriesToAdd)
                    {
                        ProductCategory productCategory = new ProductCategory()
                        {
                            CategoryId = category.CategoryId,
                            ProductId = updatedProduct.ProductId
                        };

                        _koopDbContext.ProductCategories.Add(productCategory);
                    }
                    
                    _koopDbContext.ProductCategories.RemoveRange(categoriesToRemove);
                }

                if (updatedProduct.AvailQuantity.Any())
                {
                    var currentAvailableQuantities =
                        _koopDbContext.AvailableQuantities.Where(p => p.ProductId == product.ProductId);

                    var availQuantToAdd = updatedProduct.AvailQuantity.Where(p =>
                        !currentAvailableQuantities.Select(q => q.Quantity).Contains(p.Quantity));
                    var availQuantToRemove = currentAvailableQuantities.Where(p =>
                        !updatedProduct.AvailQuantity.Select(q => q.Quantity).Contains(p.Quantity));

                    foreach (var quantity in availQuantToAdd)
                    {
                        AvailableQuantity availableQuantity = new AvailableQuantity()
                        {
                            // AvailableQuantityId = Guid.Empty,
                            ProductId = updatedProduct.ProductId,
                            Quantity = quantity.Quantity
                        };

                        _koopDbContext.AvailableQuantities.Add(availableQuantity);
                    }
                    
                    _koopDbContext.AvailableQuantities.RemoveRange(availQuantToRemove);
                }

                if (picture is not null && picture.Length > 0)
                {
                    var fileExtension = picture.FileName.Split('.');
                    if (fileExtension.Length != 2)
                    {
                        throw new Exception($"Problem z obrazkiem - za dużo kropek");
                    }

                    var fileName = $"{updatedProduct.ProductId}.{fileExtension[1]}";
                    string dirPath = "Resources/ProductImgs";
                    var fullPath = Path.Combine(dirPath, fileName);

                    // Remove files with the same name
                    var filesWithSameName = Directory.GetFiles(dirPath, $"{updatedProduct.ProductId}*");
                    foreach (var file in filesWithSameName)
                    {
                        File.Delete(file);
                    }

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        picture.CopyTo(stream);
                    }

                    updatedProduct.Picture = fullPath;
                    _koopDbContext.Products.Update(updatedProduct);
                    _koopDbContext.SaveChanges();
                }

                _koopDbContext.SaveChanges();
                problemResponse.Detail = "Dane zostały poprawnie zapisane";
                problemResponse.Status = 200;
                scope.Complete();
            }
            catch (Exception e)
            {
                problemResponse.Detail = e.Message;
                scope.Dispose();
            }
            
            return problemResponse;
        }

        public ShopRepositoryReturn RemoveProduct(IEnumerable<Product> product)
        {
            _koopDbContext.Products.RemoveRange(product);

            return ShopRepositoryReturn.RemoveProductSuccess;
        }

        public IEnumerable<AvailableQuantity> GetAvailableQuantities(Guid productId)
        {
            // AsNoTracking - prevents Entity from including Products within AvailableQuantities 
            var amountMax = _koopDbContext.Products.AsNoTracking().SingleOrDefault(p => p.ProductId == productId).AmountMax;
            var availQuantities = _koopDbContext.AvailableQuantities
                .Where(p => p.ProductId == productId && p.Quantity <= amountMax);
            //return _koopDbContext.AvailableQuantities.Where(p => p.ProductId == productId);
            return availQuantities;
        }
        
        public IEnumerable<AvailableQuantity> GetAllAvailableQuantities(Guid productId)
        {
            var availQuantities = _koopDbContext.AvailableQuantities
                .Where(p => p.ProductId == productId);
            
            return availQuantities;
        }

        public ShopRepositoryReturn UpdateAvailableQuantities(IEnumerable<AvailableQuantity>availableQuantity)
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

                    return ShopRepositoryReturn.UpdateAvailableQuantitiesSuccess;
                }
                else
                {
                    AvailableQuantity availableQuantityNew = new AvailableQuantity();
                    var availableQuantityUpdated = _mapper.Map(item, availableQuantityNew);
                    _koopDbContext.AvailableQuantities.Add(availableQuantityUpdated);

                    return ShopRepositoryReturn.AddAvailableQuantitiesSuccess;
                }
            }

            return ShopRepositoryReturn.UpdateAvailableQuantitiesNone;
        }

        public ShopRepositoryReturn RemoveAvailableQuantities(IEnumerable<AvailableQuantity> availableQuantity)
        {
            _koopDbContext.AvailableQuantities.RemoveRange(availableQuantity);

            return ShopRepositoryReturn.RemoveAvailableQuantitiesSuccess;
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
        
        public ShopRepositoryReturn UpdateProductCategories(IEnumerable<ProductCategoriesCombo> productCategoriesCombos)
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

                    return ShopRepositoryReturn.UpdateProductCategoriesSuccess;
                }
                else
                {
                    ProductCategory productCategoryNew = new ProductCategory();
                    var productCatUpdated = _mapper.Map(item, productCategoryNew);
                    _koopDbContext.ProductCategories.Add(productCatUpdated);

                    return ShopRepositoryReturn.AddProductCategoriesSuccess;
                }
            }

            return ShopRepositoryReturn.UpdateProductCategoriesNone;
        }

        public ShopRepositoryReturn RemoveProductCategories(IEnumerable<ProductCategoriesCombo> productCategoriesCombos)
        {
            IEnumerable<ProductCategory> productCategories = new List<ProductCategory>();
            var productCategoriesToRemove = _mapper.Map<IEnumerable<ProductCategoriesCombo>, IEnumerable<ProductCategory>>(productCategoriesCombos);

            _koopDbContext.ProductCategories.RemoveRange(productCategoriesToRemove);

            return ShopRepositoryReturn.RemoveProductCategoriesSuccess;
        }

        public ShopRepositoryReturn UpdateCategories(IEnumerable<Category> productCategories)
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

                    return ShopRepositoryReturn.UpdateCategoriesSuccess;
                }
                
                Category categoryNew = new Category();
                var categoryUpdated = _mapper.Map(item, categoryNew);
                _koopDbContext.Categories.Add(categoryUpdated);
                    
                return ShopRepositoryReturn.AddCategoriesSuccess;
            }

            return ShopRepositoryReturn.UpdateCategoriesNone;
        }

        public ShopRepositoryReturn RemoveCategories(IEnumerable<Category> productCategories)
        {
            _koopDbContext.Categories.RemoveRange(productCategories);

            return ShopRepositoryReturn.RemoveCategoriesSuccess;
        }

        public ShopRepositoryReturn UpdateUnits(IEnumerable<Unit> units)
        {
            foreach (var item in units)
            {
                var unitsExist =
                    _koopDbContext.Units.SingleOrDefault(p =>
                        p.UnitId == item.UnitId);

                if (unitsExist is not null)
                {
                    var unitsUpdated = _mapper.Map(item, unitsExist);

                    _koopDbContext.Units.Update(unitsUpdated);

                    return ShopRepositoryReturn.UpdateUnitsSuccess;
                }
                
                Unit unitNew = new Unit();
                var unitUpdated = _mapper.Map(item, unitNew);
                _koopDbContext.Units.Add(unitUpdated);

                return ShopRepositoryReturn.AddUnitsSuccess;
            }

            return ShopRepositoryReturn.UpdateUnitsErrNone;
        }
        
        public ShopRepositoryReturn RemoveUnits(IEnumerable<Unit> units)
        {
            _koopDbContext.Units.RemoveRange(units);

            return ShopRepositoryReturn.RemoveUnitsSuccess;
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
                    OrderedItemId = item.Products.OrderedItemId,
                    OrderId = item.Products.OrderId,
                    ProductId = item.Products.ProductId,
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

        public ShopRepositoryReturn UpdateUserOrderQuantity(Guid orderedItemId, int quantity)
        {
            var order = _koopDbContext.OrderedItems
                .Include(p => p.OrderStatus)
                .SingleOrDefault(p => p.OrderedItemId == orderedItemId);
            
            if (order is not null && order.OrderStatus.OrderStatusName.Equals(OrderStatuses.Zaplanowane.ToString()))
            {
                int quantityDiff = order.Quantity - quantity;
                order.Quantity = quantity;
                _koopDbContext.OrderedItems.Update(order);
                
                var product = _koopDbContext.Products.SingleOrDefault(p => p.ProductId == order.ProductId);
                if (product.Magazine)
                {
                    if (product.AmountInMagazine + quantityDiff >= 0)
                    {
                        product.AmountInMagazine += quantityDiff;
                    }
                }
                
                if (product.AmountMax + quantityDiff >= 0)
                {
                    product.AmountMax += quantityDiff;
                    product.Available = product.AmountMax != 0;
                    
                    return ShopRepositoryReturn.UpdateUserOrderQuantitySuccess;
                }

                return ShopRepositoryReturn.UpdateUserOrderQuantityErrTooMuch;
            }
            
            return ShopRepositoryReturn.UpdateUserOrderQuantityErrOther;
        }

        public ShopRepositoryReturn UpdateUserOrderStatus(Guid orderId, Guid userId, Guid statusId)
        {
            var userOrders = _koopDbContext.OrderedItems
                .Where(p => p.OrderId == orderId && p.CoopId == userId);

            foreach (var item in userOrders)
            {
                item.OrderStatusId = statusId;
            }

            return ShopRepositoryReturn.UpdateUserOrderStatusSuccess;
        }

        public ShopRepositoryReturn MakeOrder(Guid productId, Guid userId, int quantity)
        {
            var activeOrderId = _koopDbContext.Orders.SingleOrDefault(p => p.OrderStatus.OrderStatusName == "Szkic");

            if (activeOrderId is not null)
            {
                var orderedItem = new OrderedItem()
                {
                    OrderId = activeOrderId.OrderId,
                    CoopId = userId,
                    ProductId = productId,
                    // TODO Add default value to database table order_status
                    OrderStatusId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Quantity = quantity
                };

                _koopDbContext.OrderedItems.Add(orderedItem);
                
                var product = _koopDbContext.Products.SingleOrDefault(p => p.ProductId == productId);
                if (product.Magazine)
                {
                    if (product.AmountInMagazine - quantity >= 0)
                    {
                        product.AmountInMagazine -= quantity;
                    }
                }
                
                if (product.AmountMax - quantity >= 0)
                {
                    product.AmountMax -= quantity;
                    product.Available = product.AmountMax != 0;

                    _koopDbContext.Products.Update(product);

                    return ShopRepositoryReturn.MakeOrderSuccess;
                }

                return ShopRepositoryReturn.MakeOrderErrTooMuch;
            }

            return ShopRepositoryReturn.MakeOrderErrOther;
        }

        public ShopRepositoryReturn RemoveUserOrder(Guid orderedItemId)
        {
            var orderedItem = _koopDbContext.OrderedItems.SingleOrDefault(p => p.OrderedItemId == orderedItemId);

            if (orderedItem is not null)
            {
                _koopDbContext.OrderedItems.Remove(orderedItem);
                
                var product = _koopDbContext.Products.SingleOrDefault(p => p.ProductId == orderedItem.ProductId);
                if (product.Magazine)
                {
                    product.AmountInMagazine += orderedItem.Quantity;
                }
                
                product.AmountMax += orderedItem.Quantity;
                product.Available = product.AmountMax != 0;
                
                _koopDbContext.Products.Update(product);

                return ShopRepositoryReturn.RemoveUserOrderSuccess;
            }

            return ShopRepositoryReturn.RemoveUserOrderErrEmptyObject;
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

        public Supplier GetSupplier(Guid supplierId)
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
            
            return _koopDbContext.Suppliers.SingleOrDefault(s=> s.SupplierId == supplierId);
        }

        public IEnumerable<Product> GetProductsBySupplier(Guid supplierId)
        {
            return _koopDbContext.Products.Where(p => p.SupplierId == supplierId).ToList();
        }
        
        public void UpdateSupplier(Supplier supplier) 
        {
            _koopDbContext.Suppliers.Update(supplier);
            _koopDbContext.SaveChanges();
        }

        public void ToggleSupplierAvailability(Supplier supplier)
        {
            supplier.Available = !supplier.Available;
            _koopDbContext.Suppliers.Update(supplier);
            _koopDbContext.SaveChanges();
        }
        
        public void ToggleProductAvailability(Product product)
        {
            product.Available = !product.Available;
            _koopDbContext.Products.Update(product);
            _koopDbContext.SaveChanges();
        }
        
        public void ToggleSupplierBlocked(Supplier supplier)
        {
            supplier.Blocked = !supplier.Blocked;
            _koopDbContext.Suppliers.Update(supplier);
            _koopDbContext.SaveChanges();
        }
        
        public void ToggleProductBlocked(Product product)
        {
            product.Blocked = !product.Blocked;
            _koopDbContext.Products.Update(product);
            _koopDbContext.SaveChanges();
        }

        public void ChangeOrderStatus(Order order, OrderStatuses newStatus)
        { 
            var newStatusId = _koopDbContext.OrderStatuses.SingleOrDefault(s=>s.OrderStatusName == newStatus.ToString())?.OrderStatusId;
            
            switch (newStatus)
            {
                case OrderStatuses.Otwarte:
                    ClearBaskets();
                    // more logic to write: open shop with available suppliers & products
                    break;
                case OrderStatuses.Zamknięte:
                    //logic: close shop
                    AssignBaskets(order.OrderId);
                    break;
                case OrderStatuses.Anulowane:
                    //logic: close shop
                    break;
            }

            if (newStatusId != null)
            {
                order.OrderStatusId = (Guid) newStatusId;
                _koopDbContext.Orders.Update(order);
                _koopDbContext.SaveChanges();
            }
        }

        public void ClearBaskets()
        {
            var users = _koopDbContext.Users.Where(u=>u.BasketId != null).ToList();
            foreach (var user in users)
            {
                user.BasketId = null;
                _koopDbContext.Users.Update(user);
            }
            
            var baskets = _koopDbContext.Baskets.Where(b => b.CoopId != null).ToList();
            foreach (var b in baskets)
            {
                b.CoopId = null;
                _koopDbContext.Baskets.Update(b);
            }
            
            _koopDbContext.SaveChanges();
        }

        public void AssignBaskets(Guid orderId)
        {
            var usersIds = _koopDbContext.OrderedItems.Where(o => o.OrderId == orderId)?.Select(o=>o.CoopId).Distinct().ToList();
            // var users = _koopDbContext.Users.Where(u=>u.BasketId != null).ToList();
            var baskets = _koopDbContext.Baskets.ToList();
            
            int i = 0;
            foreach (var id in usersIds)
            {
                var user = _koopDbContext.Users.SingleOrDefault(u => u.Id == id);
                if (user != null)
                {
                    if (i >= baskets.Count)
                    {
                        Basket newBasket = new Basket()
                        {
                            BasketName = $"basket{i + 1}"
                        };
                        _koopDbContext.Baskets.Add(newBasket);
                        _koopDbContext.SaveChanges();
                    }
                
                    user.BasketId = baskets[i].BasketId;
                    baskets[i].CoopId = user.Id;
                    i++;
                }
            }
        }
    }
}