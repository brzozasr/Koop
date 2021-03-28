using System;
using System.Linq;
using Koop.Models;
using Koop.Models.Repositories;
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
        
        [HttpGet("supplier/{abbr}")]
        public IActionResult Supplier(string abbr)
        {
            return Ok(_uow.Repository<Supplier>().GetAll().SingleOrDefault(s => s.SupplierAbbr.ToLower() == abbr));
            // return _uow.Repository<Supplier>().GetAll().Include(p => p.Products)
            //     .SingleOrDefaultAsync(p => p.SupplierAbbr == "CHOR");
        }

        [HttpGet("supplier/{abbr}/edit")]
        public IActionResult EditSupplier(string abbr)
        {
            return Ok(_uow.Repository<Supplier>().GetAll().SingleOrDefault(s => s.SupplierAbbr.ToLower() == abbr));
        }

        [HttpGet("allsuppliers")]
        public IActionResult AllSuppliers()
        {
            return Ok(_uow.Repository<Supplier>().GetAll());
        }
        
        [HttpGet("cooperator/{firstname}+{lastname}/history")]
        public IActionResult CoOrderHistoryView(string firstName, string lastName)
        {
            return Ok(_uow.Repository<CoopOrderHistoryView>().GetAll().Where(s=>s.FirstName.ToLower() == firstName && s.LastName.ToLower() == lastName));
        }
        
        [HttpGet("order/baskets")]
        public IActionResult BasketName()
        {
            return Ok(_uow.Repository<Basket>().GetAll());
        }
    }
}