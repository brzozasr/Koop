using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Koop.Models.Repositories;
using Koop.Models.RepositoryModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Koop.Controllers
{
    public class TestController : Controller
    {
        private IGenericUnitOfWork _uow;

        public TestController(IGenericUnitOfWork genericUnitOfWork)
        {
            _uow = genericUnitOfWork;
        }
        
        public IActionResult Index()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }

        public IActionResult NoAuth()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }
        
        [Authorize]
        public IActionResult Auth()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }
        
        [Authorize(Policy = "Szymek")]
        public IActionResult AuthUserName()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }
        
        [Authorize(Roles = "Koty")]
        public IActionResult AuthRole()
        {
            return Ok(new
            {
                Message = "It works",
                Time = DateTime.Now
            });
        }

        [HttpGet]
        public IActionResult Products(string orderBy="name", int start=1, int count=10, string orderDir="asc")
        {
            orderBy = orderBy.ToLower();
            Expression < Func<ProductsShop, object> > order = orderBy switch
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
    }
}