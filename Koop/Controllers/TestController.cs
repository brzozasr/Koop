using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using Koop.models;
using Koop.Models;
using Koop.Models.Repositories;
using Koop.Models.RepositoryModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Koop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private IGenericUnitOfWork _uow;

        public TestController(IGenericUnitOfWork genericUnitOfWork)
        {
            _uow = genericUnitOfWork;
        }
        
        [AllowAnonymous]
        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }

        [AllowAnonymous]
        [HttpGet("NoAuth")]
        public IActionResult NoAuth()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }
        
        [Authorize]
        [HttpGet("Auth")]
        public IActionResult Auth()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }
        
        [Authorize(Policy = "Szymek")]
        [HttpGet("AuthUserName")]
        public IActionResult AuthUserName()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }
        
        [Authorize(Roles = "Koty")]
        [HttpGet("AuthRole")]
        public IActionResult AuthRole()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }

        [HttpGet("products")]
        public IActionResult Products(string orderBy = "name", int start = 1, int count = 10, string orderDir = "asc")
        {
            orderBy = orderBy.ToLower();
            Expression<Func<ProductsShop, object>> order = orderBy switch
            {
                "name" => p => p.ProductName,
                "price" => p => p.Price,
                "blocked" => p => p.Blocked,
                "available" => p => p.Available,
                "unit" => p => p.Unit,
                "amountmax" => p => p.AmountMax,
                "supplierabbr" => p => p.SupplierAbbr,
                _ => p => p.ProductName
            };

            OrderDirection direction = orderDir switch
            {
                "asc" => OrderDirection.Asc,
                "desc" => OrderDirection.Desc,
                _ => OrderDirection.Asc
            };

            return Ok(_uow.ShopRepository().GetProductsShop(order, start, count, direction));
        }

        [HttpGet("product")]
        public IActionResult Product(Guid productId)
        {
            return Ok(_uow.ShopRepository().GetProductById(productId));
        }

        [HttpPost("product/edit")]
        public IActionResult ProductEdit(Guid productId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("product/categories")]
        public IActionResult GetProductCatgeories(Guid productId)
        {
            return Ok(_uow.ShopRepository().GetProductCategories(productId));
        }
        
        [HttpPost("product/categories/update")]
        public IActionResult UpdateCategories(IEnumerable<ProductCategoriesCombo> productCategoriesCombos)
        {
            _uow.ShopRepository().UpdateProductCategories(productCategoriesCombos);

            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Table ProductCategories updated successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }
        
        [HttpDelete("product/categories/remove")]
        public IActionResult RemoveCategories(IEnumerable<ProductCategoriesCombo> productCategoriesCombos)
        {
            _uow.ShopRepository().RemoveProductCategories(productCategoriesCombos);

            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Entries of ProductCategories were removed successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }
        
        [HttpGet("product/availQuantities")]
        public IActionResult GetProductAvailQuantities(Guid productId)
        {
            return Ok(_uow.ShopRepository().GetAvailableQuantities(productId));
        }

        [HttpPost("product/availQuantities/update")]
        public IActionResult UpdateAvailQuantities(IEnumerable<AvailableQuantity> availableQuantities)
        {
            _uow.ShopRepository().UpdateAvailableQuantities(availableQuantities);

            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Table AvailableQuantities updated successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpDelete("product/availQuantities/remove")]
        public IActionResult RemoveAvailQuantities(IEnumerable<AvailableQuantity> availableQuantities)
        {
            _uow.ShopRepository().RemoveAvailableQuantities(availableQuantities);
            
            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Entries of AvailableQuantities were removed successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            return Ok(_uow.Repository<Category>().GetAll());
        }

        [HttpPost("categories/update")]
        public IActionResult UpdateCategories(IEnumerable<Category> categories)
        {
            _uow.ShopRepository().UpdateCategories(categories);
            
            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Table Categories updated successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpDelete("categories/remove")]
        public IActionResult RemoveCategories(IEnumerable<Category> categories)
        {
            _uow.ShopRepository().RemoveCategories(categories);
            
            try
            {
                _uow.SaveChanges();
                return Ok(new {Message = "Entries of Categories were removed successfully."});
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpGet("cooporder")]
        public IActionResult CoopOrder(Guid coopId, Guid orderId)
        {
            return Ok(_uow.ShopRepository().GetCooperatorOrders(coopId, orderId));
        }

        [HttpGet("supplier/{abbr}")]
        public IActionResult Supplier(string abbr)
        {
            return Ok(_uow.ShopRepository().GetSupplier(abbr));
        }
        
        

        
        [HttpGet("supplier/{abbr}/edit")]
        public IActionResult EditSupplier(string abbr)
        {
            return Ok(_uow.ShopRepository().GetSupplier(abbr));
        }
        
        //TODO EWA: refactor!

        // [HttpGet("allsuppliers")]
        // public IActionResult AllSuppliers()
        // {
        //     return Ok(_uow.Repository<Supplier>().GetAll());
        // }
        
        [HttpGet("cooperator/{firstname}+{lastname}/history")]
        public IActionResult UserOrdersHistoryView(string firstName, string lastName)
        {
            return Ok(_uow.ShopRepository().GetUserOrders(firstName, lastName));
        }
        
        [HttpGet("order/baskets")]
        public IActionResult BasketName()
        {
            return Ok(_uow.ShopRepository().GetBaskets());
        }
        
        [HttpGet("bigorders")]
        public IActionResult BigOrders()
        {
            return Ok(_uow.Repository<Order>().GetAll());
        }
    }
}